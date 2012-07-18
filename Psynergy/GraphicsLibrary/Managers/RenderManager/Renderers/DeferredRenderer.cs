using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Camera;
using Psynergy.Input;
using SkinnedModel;

namespace Psynergy.Graphics
{
    public class DeferredRenderer : PsynergyRenderer
    {
        #region Utilities
        // Quad renderer to be able to render to screen
        private QuadRenderer m_QuadRenderer;
        #endregion

        #region Render targets ( modify to render capture classes later :) )
        private RenderTargetBinding[] m_GBuffer = null;
        private RenderTarget2D m_ColorTarget = null;
        private RenderTarget2D m_NormalTarget = null;
        private RenderTarget2D m_DepthTarget = null;
        private RenderTarget2D m_SecondaryDepthTarget = null;
        private RenderTarget2D m_LightTarget = null;
        private RenderTarget2D m_ShadowDepthTarget = null;
        #endregion

        #region Effects
        private Effect m_ClearGBufferEffect = null;
        #endregion

        #region Graphics Techniques
        #region Shadow Mapping
        // Depth texture parameters
        private int m_ShadowMapSize = 2048;

        // Shadow light view and projection
        private Matrix m_ShadowView;
        private Matrix m_ShadowProjection;
        #endregion
        #endregion

        #region New Mesh Rendering Format
        private BlendState m_LightAddBlendState = null;

        // Overall meshes
        private List<Mesh> m_Meshes = new List<Mesh>();

        // Visible sub meshes
        private List<AbstractMesh>[] m_VisibleMeshes = new List<AbstractMesh>[(int)(ERenderQueue.Count)];
        
        // Shadow meshes and visible shadow meshes
        private List<Mesh> m_ShadowedMeshes = new List<Mesh>();
        private List<AbstractMesh>[] m_VisibleShadowedMeshes = new List<AbstractMesh>[(int)(ERenderQueue.Count)];
        #endregion

        #region Post Processing
        private ToneMappingProperties m_ToneMappingProperties = new ToneMappingProperties();

        private DepthOfField m_DepthOfFieldPostProcessor = null;
        private Bloom m_BloomPostProcessor = null;
        private MLAA m_MLAAProcessor = null;
        private FilmGrain m_FilmGrainProcessor = null;
        private SSAO m_SSAOProcessor = null;
        private FXAA m_FXAAProcessor = null;

        // Effects
        private Effect m_GaussianEffect = null;
        private Effect m_DepthOfFieldEffect = null;
        private Effect m_BloomEffect = null;
        private Effect m_MergeEffect = null;
        private Effect m_EdgeDetectionEffect = null;
        private Effect m_MLAAEffect = null;
        private Effect m_FilmGrainEffect = null;
        private Effect m_SSAOEffect = null;
        private Effect m_SSAOBlurEffect = null;
        private Effect m_SSAOMergeEffect = null;
        private Effect m_FXAAEffect = null;
        #endregion

        public DeferredRenderer(ContentManager contentManager, GraphicsDeviceManager graphicsDevice) : base(contentManager, graphicsDevice)
        {
            // Create a quad renderer
            m_QuadRenderer = new QuadRenderer();
        }

        public override void Initialise()
        {
            base.Initialise();

            if (GraphicsDevice != null)
            {
                // Render Targets
                m_ShadowDepthTarget = new RenderTarget2D(GraphicsDevice, m_ShadowMapSize, m_ShadowMapSize, false, SurfaceFormat.HalfVector2, DepthFormat.Depth24);

                // Shadow multiplier value
                ShadowMult = 0.5f;

                // Custom light blending state
                m_LightAddBlendState = new BlendState()
                {
                    
                    AlphaSourceBlend = Blend.One,
                    ColorSourceBlend = Blend.One,
                    AlphaDestinationBlend = Blend.One,
                    ColorDestinationBlend = Blend.One,
                };

                // Create these post processors up front
                m_DepthOfFieldPostProcessor = new DepthOfField(GraphicsDevice);
                m_BloomPostProcessor = new Bloom(GraphicsDevice);
                m_FilmGrainProcessor = new FilmGrain(GraphicsDevice);
            }

            //create render queues
            for (int index = 0; index < (int)ERenderQueue.Count; index++)
            {
                m_VisibleMeshes[index] = new List<AbstractMesh>();
                m_VisibleShadowedMeshes[index] = new List<AbstractMesh>();
            }
        }

        protected override void SetupRenderTargets()
        {
            base.SetupRenderTargets();

            // Create GBuffer Render Target binding
            m_GBuffer = new RenderTargetBinding[4];

            // Rebuild the render targets
            m_ColorTarget = new RenderTarget2D(GraphicsDevice, (int)m_ScreenSize.X, (int)m_ScreenSize.Y, true, SurfaceFormat.Color, DepthFormat.Depth24);
            m_NormalTarget = new RenderTarget2D(GraphicsDevice, (int)m_ScreenSize.X, (int)m_ScreenSize.Y, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            m_DepthTarget = new RenderTarget2D(GraphicsDevice, (int)m_ScreenSize.X, (int)m_ScreenSize.Y, false, SurfaceFormat.Single, DepthFormat.Depth24Stencil8);
            m_SecondaryDepthTarget = new RenderTarget2D(GraphicsDevice, (int)m_ScreenSize.X, (int)m_ScreenSize.Y, false, SurfaceFormat.HalfVector2, DepthFormat.Depth24Stencil8);
            m_LightTarget = new RenderTarget2D(GraphicsDevice, (int)m_ScreenSize.X, (int)m_ScreenSize.Y, true, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            // Add render target bindings
            m_GBuffer[0] = new RenderTargetBinding(m_ColorTarget);
            m_GBuffer[1] = new RenderTargetBinding(m_NormalTarget);
            m_GBuffer[2] = new RenderTargetBinding(m_DepthTarget);
            m_GBuffer[3] = new RenderTargetBinding(m_SecondaryDepthTarget);
        }

        public override void Reset()
        {
            base.Reset();

            // Reset Lights
            foreach (Light light in m_Lights)
            {
                // Setup Light
                SetupLight(light);
            }

            // Reset current mesh effects
            foreach (Mesh mesh in m_Meshes)
            {
                // Setup Mesh
                SetupMesh(mesh);
            }

            // Reset the post processing render captures
            if ( m_DepthOfFieldPostProcessor != null )
                m_DepthOfFieldPostProcessor.Reset();

            if ( m_BloomPostProcessor != null )
                m_BloomPostProcessor.Reset();

            if ( m_FilmGrainProcessor != null )
                m_FilmGrainProcessor.Reset();

            if ( m_MLAAProcessor != null )
                m_MLAAProcessor.Reset();
        }

        public override void Load()
        {
            base.Load();

            if (GraphicsDevice != null)
            {
                // Effects
                m_ClearGBufferEffect = RenderManager.Instance.LoadEffect("Shaders/Deferred Rendering/ClearGBuffer");

                // Post Processing 
                m_GaussianEffect = RenderManager.Instance.GetEffect("GaussianBlurPostProcessor");
                m_DepthOfFieldEffect = RenderManager.Instance.GetEffect("DOFPostProcessor");
                m_BloomEffect = RenderManager.Instance.GetEffect("BloomPostProcessor");
                m_MergeEffect = RenderManager.Instance.GetEffect("MergeBloomPostProcessor");
                m_EdgeDetectionEffect = RenderManager.Instance.GetEffect("EdgeDetectionPostProcessor");
                m_MLAAEffect = RenderManager.Instance.GetEffect("MLAAPostProcessor");
                m_FilmGrainEffect = RenderManager.Instance.GetEffect("FilmMappingPostProcessor");
                m_SSAOEffect = RenderManager.Instance.GetEffect("SSAO");
                m_SSAOBlurEffect = RenderManager.Instance.GetEffect("SSAOBlur");
                m_SSAOMergeEffect = RenderManager.Instance.GetEffect("SSAOMerge");
                m_FXAAEffect = RenderManager.Instance.GetEffect("FXAA");

                // Post Processor
                m_MLAAProcessor = new MLAA(GraphicsDevice, m_MLAAEffect, m_EdgeDetectionEffect);
                m_SSAOProcessor = new SSAO(GraphicsDevice, m_SSAOEffect, m_SSAOBlurEffect, m_SSAOMergeEffect);
                m_FXAAProcessor = new FXAA(GraphicsDevice, m_FXAAEffect);

                // These post processors were created up front so just assign the effect so them
                m_DepthOfFieldPostProcessor.Effect = m_DepthOfFieldEffect;
                m_DepthOfFieldPostProcessor.BlurEffect = m_GaussianEffect;
                m_BloomPostProcessor.Effect = m_BloomEffect;
                m_BloomPostProcessor.BlurEffect = m_GaussianEffect;
                m_BloomPostProcessor.MergeEffect = m_MergeEffect;
                m_FilmGrainProcessor.Effect = m_FilmGrainEffect;
            }
        }

        #region SetGBuffer
        private void SetGBuffer()
        {
            if (GraphicsDevice != null)
            {
                GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
                GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
                GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;
                GraphicsDevice.SamplerStates[3] = SamplerState.PointClamp;

                GraphicsDevice.SetRenderTargets(m_GBuffer);
            }
        }
        #endregion

        #region ResolveGBuffer
        private void ResolveGBuffer()
        {
            if (GraphicsDevice != null)
                GraphicsDevice.SetRenderTargets(null);
        }
        #endregion

        #region Clear GBuffer
        private void ClearGBuffer()
        {
            if ((GraphicsDevice != null) && (m_ClearGBufferEffect != null))
            {
                GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
                GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
                GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

                // Apply the effect
                m_ClearGBufferEffect.CurrentTechnique.Passes[0].Apply();

                // Render to the whole screen
                m_QuadRenderer.RenderQuad(GraphicsDevice, -Vector2.One, Vector2.One);
            }
        }
        #endregion

        #region Render Loop
        public override void Begin()
        {
            if (GraphicsDevice != null)
            {
                // Initial rendering state
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                GraphicsDevice.BlendState = BlendState.Opaque;
            }
        }

        public override void Draw(GameTime deltaTime)
        {
            if (GraphicsDevice != null)
            {
                // Determine what sub-meshes to render
                CullVisibleMeshes();

                // Set the GBuffer
                SetGBuffer();

                // Clear GBuffer
                ClearGBuffer();

                // Make sure sampler is set to anisotropic for rendering to diffuse and specular targets
                GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;

                // Draw the scene Render Nodes
                RenderToGbuffer(deltaTime);

                // Resolve the GBuffer
                ResolveGBuffer();

                // DRAW REFLECTIONS BEFORE LIGHTING
                DrawReflections(deltaTime);

                if (EnableShadowMapping)
                {
                    // FOR NOW SHADOW MAPS ARE STILL SEPERATE PASS
                    // THIS IS OPTIMISED NOW THOUGH! HOPEFULLY LESS OBJECTS IN THE SHADOW PASS!
                    DrawShadowDepthMap(deltaTime);
                }

                // Draw lights
                DrawLights(deltaTime);

                // Combine Final Image
                CombineFinal(deltaTime);
            }
        }

        public override void End()
        {
            base.End();

            float spritescaler = 0.2f;

            //DebugRender.Instance.AddTexture2D(lightTexture, new Vector2(0, 0), spritescaler);
            //DebugRender.Instance.AddTexture2D(specularTexture, new Vector2(0 + (lightTexture.Width * spritescaler), 0), spritescaler);
            //DebugRender.Instance.AddTexture2D(normalTexture, new Vector2((0 + (lightTexture.Width * spritescaler) + (specularTexture.Width * spritescaler)), 0), spritescaler);

            // Draw buffers for debugging
            //DebugRender.Instance.AddTexture2D(m_ColorTarget, new Vector2(100, 0), spritescaler);
            //DebugRender.Instance.AddTexture2D(m_ReflectionTarget, new Vector2(100 + (m_ColorTarget.Width * spritescaler), 0), spritescaler);
            //DebugRender.Instance.AddTexture2D(m_RefractionTarget, new Vector2((100 + (m_ColorTarget.Width * spritescaler) + (m_NormalTarget.Width * spritescaler)), 0), spritescaler);
            DebugRender.Instance.AddTexture2D(m_SecondaryDepthTarget, new Vector2((100 + (m_ColorTarget.Width * spritescaler) + (m_NormalTarget.Width * spritescaler) + (m_DepthTarget.Width * spritescaler)), 0), spritescaler);
        }
        #endregion

        #region Render Management
        private void CullVisibleMeshes()
        {
            Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

            // Clear the visible mesh lists
            for (int index = 0; index < (int)(ERenderQueue.Count); index++)
            {
                List<AbstractMesh> visibleMesh = m_VisibleMeshes[index];
                visibleMesh.Clear();

                // Clear shadow list
                visibleMesh = m_VisibleShadowedMeshes[index];
                visibleMesh.Clear();
            }

            if (camera != null)
            {
                // Sort overall meshes into the visible buffer
                for (int index = 0; index < m_Meshes.Count; index++)
                {
                    Mesh mesh = m_Meshes[index];

                    // First check if the mesh is even active
                    if (mesh.Active)
                    {
                        // Cull meshes by camera frustum
                        if (!mesh.IgnoreCameraCulling && !camera.Frustum.Intersects(mesh.GlobalBoundingBox))
                            continue;

                        for (int i = 0; i < mesh.SubMeshes.Count; i++)
                        {
                            SubMesh subMesh = mesh.SubMeshes[i];

                            // Add sub meshes if they are intersected by the camera
                            if (mesh.IgnoreCameraCulling || camera.Frustum.Intersects(subMesh.GlobalBoundingBox))
                                m_VisibleMeshes[(int)subMesh.RenderQueue].Add(subMesh);
                        }
                    }
                }

                // Now sort the shadow meshes into a seperate visible buffer
                for (int index = 0; index < m_ShadowedMeshes.Count; index++)
                {
                    Mesh mesh = m_ShadowedMeshes[index];

                    // First check if the mesh is even active
                    if (mesh.Active)
                    {
                        // Cull meshes by camera frustum
                        if (!mesh.IgnoreCameraCulling && !camera.Frustum.Intersects(mesh.GlobalBoundingBox))
                            continue;

                        for (int i = 0; i < mesh.SubMeshes.Count; i++)
                        {
                            SubMesh subMesh = mesh.SubMeshes[i];

                            // Add sub meshes if they are intersected by the camera
                            if (mesh.IgnoreCameraCulling || camera.Frustum.Intersects(subMesh.GlobalBoundingBox))
                                m_VisibleShadowedMeshes[(int)subMesh.RenderQueue].Add(subMesh);
                        }
                    }
                }
            }
            else
            {
            }
        }
        #endregion

        #region Rendering Functions
        private void RenderToGbuffer(GameTime deltaTime)
        {
            if (GraphicsDevice != null)
                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

            List<AbstractMesh> meshes = m_VisibleMeshes[(int)ERenderQueue.Default];

            foreach (AbstractMesh mesh in meshes)
            {
                mesh.RenderToGBuffer(camera, GraphicsDevice);
            }
        }

        private void ReconstructShading(GameTime deltaTime)
        {
            Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

            List<AbstractMesh> meshes = m_VisibleMeshes[(int)ERenderQueue.Default];

            foreach (AbstractMesh mesh in meshes)
            {
                mesh.ReconstructShading(deltaTime, camera, camera.View, camera.Projection, GraphicsDevice);
            }

            meshes = m_VisibleMeshes[(int)ERenderQueue.Reflects];

            // Temporary while i get water working with reflections
            foreach (AbstractMesh mesh in meshes)
            {
                mesh.ReconstructShading(deltaTime, camera, camera.View, camera.Projection, GraphicsDevice);
            }
        }

        private void DrawOpaqueObjects(GameTime deltaTime)
        {
            Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

            List<AbstractMesh> meshes = m_VisibleMeshes[(int)ERenderQueue.SkipGbuffer];
            for (int index = 0; index < meshes.Count; index++)
            {
                AbstractMesh visibleMesh = meshes[index];
                visibleMesh.GenericRender(camera, GraphicsDevice);
            }
        }

        private void DrawReflectionObjects(GameTime deltaTime)
        {
            Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

            List<AbstractMesh> meshes = m_VisibleMeshes[(int)ERenderQueue.Reflects];
            for (int index = 0; index < meshes.Count; index++)
            {
                AbstractMesh visibleMesh = meshes[index];

                List<AbstractMesh> reflectionMeshes = new List<AbstractMesh>();
                reflectionMeshes.AddRange(m_VisibleMeshes[(int)ERenderQueue.Default]);
                reflectionMeshes.AddRange(m_VisibleMeshes[(int)ERenderQueue.SkipGbuffer]);

                visibleMesh.ReflectionRender(deltaTime, reflectionMeshes, camera, GraphicsDevice);
            }
        }

        private void DrawShadowMap(Matrix lightViewProjection)
        {
            Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

            List<AbstractMesh> meshes = m_VisibleShadowedMeshes[(int)ERenderQueue.Default];
            foreach (AbstractMesh mesh in meshes)
            {
                if (!mesh.CastShadows)
                    continue;

                mesh.RenderShadowMap(ref lightViewProjection, GraphicsDevice);
            }
        }
        #endregion

        #region Reflection Drawing
        private void DrawReflections(GameTime deltaTime)
        {
            // Draw Reflection objects
            DrawReflectionObjects(deltaTime);
        }
        #endregion

        #region Light Management
        private void DrawLights(GameTime deltaTime)
        {
            if (GraphicsDevice != null)
            {
                // Set light map target
                GraphicsDevice.SetRenderTargets(m_LightTarget);

                // Clear screen
                GraphicsDevice.Clear(Color.Transparent);

                // Set Blend State
                GraphicsDevice.BlendState = BlendState.AlphaBlend;
                GraphicsDevice.DepthStencilState = DepthStencilState.None;
                GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                // Go through the lights
                foreach (Light light in m_Lights)
                {
                    Type lightType = light.GetType();

                    if (lightType == typeof(DirectionalLight))
                    {
                        DirectionalLight directionalLight = (light as DirectionalLight);

                        // Update Parameters
                        directionalLight.ViewProjection = (m_ShadowView * m_ShadowProjection);
                    }
                    else if (lightType == typeof(PointLight))
                    {
                        PointLight pointLight = (light as PointLight);

                        // NO UPDATE PARAMETERS ATM =]
                    }

                    // Draw Light
                    light.Draw(deltaTime);
                }

                // Clear target
                GraphicsDevice.SetRenderTarget(null);
            }
        }

        #region Final Combine
        private void CombineFinal(GameTime deltaTime)
        {
            // Set light map target
            GraphicsDevice.SetRenderTarget(m_MainRenderTarget);

            // Clear screen
            GraphicsDevice.Clear(Color.Black);

            // Set new graphics states
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            // Reconstruct scene shading from GBuffer
            ReconstructShading(deltaTime);

            // Draw Opaque Objects
            DrawOpaqueObjects(deltaTime);
        }
        #endregion
        #endregion

        #region PostProcessing
        protected override Texture2D PostProcess(Texture2D inTexture)
        {
            Texture2D texture = base.PostProcess(inTexture);

            if (texture != null)
            {
                // SSAO ( TEST ATM )
                //texture = SSAO(texture);

                if (m_FXAAProcessor.Enabled)
                    texture = FXAA(texture);

                if (m_FilmGrainProcessor.Enabled)
                    texture = FilmGrain(texture);

                if (m_BloomPostProcessor.Enabled)
                    texture = Bloom(texture);

                if (m_DepthOfFieldPostProcessor.Enabled)
                    texture = DepthOfField(texture);

                //texture = MLAA(texture);
            }

            // Send post processed texture back
            return texture;
        }

        private Texture2D FXAA(Texture2D texture)
        {
            return DrawPostProcess(m_FXAAProcessor, texture);
        }

        private Texture2D FilmGrain(Texture2D texture)
        {
            return DrawPostProcess(m_FilmGrainProcessor, texture);
        }

        private Texture2D SSAO(Texture2D texture)
        {
            // Store depth map
            m_SSAOProcessor.NormalMap = m_NormalTarget;
            m_SSAOProcessor.DepthMap = m_SecondaryDepthTarget;

            if (InputManager.Instance.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
                m_SSAOProcessor.DistanceScale += 100;

            if (InputManager.Instance.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
                m_SSAOProcessor.DistanceScale -= 100;

            if (InputManager.Instance.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
                m_SSAOProcessor.SampleRadius -= 0.5f;

            if (InputManager.Instance.KeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
                m_SSAOProcessor.SampleRadius += 0.5f;

            // Draw the post process 
            texture = DrawPostProcess(m_SSAOProcessor, texture);

            return texture;
        }

        private Texture2D DepthOfField(Texture2D texture)
        {
            Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

            if (camera != null)
            {
                // Set depth of field effect variables
                m_DepthOfFieldEffect.Parameters["xMaxDepth"].SetValue(camera.FarPlane);
            }

            // Set depth map and a copy of the un blurred image
            m_DepthOfFieldPostProcessor.DepthMap = m_SecondaryDepthTarget;
            m_DepthOfFieldPostProcessor.UnBlurred = texture;

            // Draw the post process 
            texture = DrawPostProcess(m_DepthOfFieldPostProcessor, texture);

            // Return the texture thats had depth of field applied to it
            return texture;
        }

        private Texture2D Bloom(Texture2D texture)
        {
            // Draw the post process 
            texture = DrawPostProcess(m_BloomPostProcessor, texture);

            return texture;
        }

        private Texture2D MLAA(Texture2D texture)
        {
            m_MLAAProcessor.DepthMap = m_DepthTarget;

            // Draw the post process 
            texture = DrawPostProcess(m_MLAAProcessor, texture);

            return texture;
        }

        private Texture2D DrawPostProcess(AbstractPostProcessor processor, Texture2D mainTarget)
        {
            Texture2D texture = mainTarget;

            if (mainTarget != null)
            {
                if (processor != null)
                {
                    // Perform postprocessing with the render of the scene
                    processor.Draw(mainTarget);

                    // Save texture back
                    texture = processor.FinalImage;
                }
            }

            return texture;
        }
        #endregion

        #region Graphics Techniques
        #region ShadowMapping
        private void DrawShadowDepthMap(GameTime deltaTime)
        {
            // Set the render target 
            GraphicsDevice.SetRenderTarget(m_ShadowDepthTarget);

            // Clear the render target to 1 (infinite depth)
            GraphicsDevice.Clear(Color.White);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            // Calculate view and projection matrices for the "Light" Shadows are being calculated for
            //m_ShadowView = Matrix.CreateLookAt(m_ShadowLightPosition, m_ShadowLightTarget, Vector3.Up);
            //m_ShadowProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(m_Camera.FOV), 1, m_Camera.NearPlane, m_ShadowFarPlane);

            Vector3 lightDir = new Vector3(-0.5f, 0.5f, 0.0f);
            m_ShadowProjection = CreateLightOrthographicMatrix(lightDir);

            // Render shadow map
            DrawShadowMap(m_ShadowView * m_ShadowProjection);

            // Make sure render cull mode is changed back again
            RenderManager.Instance.SetCullMode(CullMode.CullCounterClockwiseFace);

            // Unset the render targets
            GraphicsDevice.SetRenderTarget(null);
        }

        private Matrix CreateLightOrthographicMatrix(Vector3 lightDir)
        {
            Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);
            Matrix toRet = Matrix.Identity;

            if (camera != null)
            {
                toRet = camera.Transform;

                // Matrix with that will rotate in points the direction of the light
                Matrix lightRotation = Matrix.CreateLookAt(Vector3.Zero,
                                                           -lightDir,
                                                           Vector3.Up);

                if (camera.ShadowFrustum != null)
                {
                    // Get the corners of the frustum
                    Vector3[] frustumCorners = camera.ShadowFrustum.GetCorners();

                    // Transform the positions of the corners into the direction of the light
                    for (int i = 0; i < frustumCorners.Length; i++)
                        frustumCorners[i] = Vector3.Transform(frustumCorners[i], lightRotation);

                    // Find the smallest box around the points
                    BoundingBox lightBox = BoundingBox.CreateFromPoints(frustumCorners);

                    Vector3 boxSize = (lightBox.Max - lightBox.Min);
                    Vector3 halfBoxSize = (boxSize * 0.5f);

                    // The position of the light should be in the center of the back
                    // pannel of the box. 
                    Vector3 lightPos = (lightBox.Min + halfBoxSize);
                    lightPos.Z = lightBox.Min.Z;

                    // We need the position back in world coordinates so we transform 
                    // the light position by the inverse of the lights rotation
                    lightPos = Vector3.Transform(lightPos, Matrix.Invert(lightRotation));

                    // Create the view matrix for the light
                    m_ShadowView = Matrix.CreateLookAt(lightPos, (lightPos - lightDir), Vector3.Up);

                    // Create the projection matrix for the light
                    // The projection is orthographic since we are using a directional light
                    toRet = Matrix.CreateOrthographic(boxSize.X, boxSize.Y, -boxSize.Z, boxSize.Z);
                }
            }

            return toRet;
        }
        #endregion
        #endregion

        #region Defer functions
        public override void DeferRenderable(RenderNode renderable)
        {
            // Only accepts models right now for this renderer
            Type type = renderable.GetType();

            if ( (type == typeof(ModelNode)) || (type.IsSubclassOf(typeof(ModelNode))))
            {
                ModelNode modelNode = (renderable as ModelNode);
                Mesh mesh = modelNode.Mesh;

                if (mesh != null)
                {
                    // Setup mesh
                    SetupMesh(mesh);

                    // Add Mesh
                    m_Meshes.Add(mesh);

                    if (mesh.SubMeshes.Count > 0)
                    {
                        // FOR NOW ADD ALL SUB MESHES TO THE RENDER LIST ( NOT OPTIMISED YET )
                        for (int i = 0; i < mesh.SubMeshes.Count; i++)
                        {
                            SubMesh subMesh = mesh.SubMeshes[i];

                            //if (camera.Frustum.Intersects(subMesh.GlobalBoundingBox))
                            //{
                            m_VisibleMeshes[(int)subMesh.RenderQueue].Add(subMesh);
                            //}
                        }
                    }
                    else
                    {
                        // Otherwise just add the mesh as there are no sub meshses
                        m_VisibleMeshes[(int)mesh.RenderQueue].Add(mesh);
                    }
                }
            }
        }

        public override void DeferLight(Light light)
        {
            if (light != null)
            {
                if (!m_Lights.Contains(light))
                {
                    // SetupLight
                    SetupLight(light);

                    // Add light
                    m_Lights.Add(light);
                }
            }
        }
        #endregion

        #region Light Setup 
        /// <summary>
        /// Call this for every light you create, so it sets the GBuffer textures only once
        /// </summary>
        /// <param name="mesh"></param>
        private void SetupLight(Light light)
        {
            Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

            light.ColorMap = m_ColorTarget;
            light.NormalMap = m_NormalTarget;
            light.DepthMap = m_DepthTarget;
            light.ShadowMap = m_ShadowDepthTarget;
            light.HalfPixel = m_HalfPixel;
            light.CastShadow = EnableShadowMapping;
        }
        #endregion

        #region Mesh Setup
        /// <summary>
        /// Call this for every mesh you create, so it sets the GBuffer textures only once
        /// </summary>
        /// <param name="mesh"></param>
        private void SetupMesh(Mesh mesh)
        {
            Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

            if (mesh.SubMeshes.Count > 0)
            {
                foreach (SubMesh subMesh in mesh.SubMeshes)
                {
                    if (subMesh.RenderEffect != null)
                    {
                        subMesh.RenderEffect.SetLightBuffer(m_LightTarget);
                        subMesh.RenderEffect.SetLightBufferPixelSize(m_HalfPixel);
                        subMesh.RenderEffect.SetAmbientColor(AmbientLighting);
                        subMesh.RenderEffect.SetFogProperties(m_FogProperties);
                        subMesh.RenderEffect.SetToneMappingProperties(m_ToneMappingProperties);
                        subMesh.RenderEffect.EnableLighting(subMesh.MetaData.EnableLighting);
                        subMesh.RenderEffect.EnableNormalMap(subMesh.MetaData.UseNormalMap);
                        subMesh.RenderEffect.SetDepthBuffer(m_SecondaryDepthTarget);

                        if (camera != null)
                            subMesh.RenderEffect.SetFarClip(camera.FarPlane);
                    }
                }
            }

            // If it is shadowed add it to the shadow buffer ( if it hasn't already )
            if (mesh.CastShadows)
                DeferShadowMesh(mesh);
        }

        private void DeferShadowMesh(Mesh mesh)
        {
            if (mesh != null)
            {
                if (!m_ShadowedMeshes.Contains(mesh))
                    m_ShadowedMeshes.Add(mesh);
            }
        }
        #endregion

        #region Graphics Setters
        #region Rendering Technique Setters
        public override void UseShadows(bool use)
        {
            Console.WriteLine("[MESSAGE] - SHADOW MAPS USED = " + use.ToString());
            EnableShadowMapping = use;

            // Setup lights
            foreach (Light light in m_Lights)
                SetupLight(light);
        }
        #endregion

        public override void SetFogProperties(FogProperties properties)
        {
            if (properties != null)
            {
                base.SetFogProperties(properties);

                // Reset current mesh effects with new fog settings
                foreach (Mesh mesh in m_Meshes)
                {
                    if (mesh.SubMeshes.Count > 0)
                    {
                        foreach (SubMesh subMesh in mesh.SubMeshes)
                        {
                            if (subMesh.RenderEffect != null)
                                subMesh.RenderEffect.SetFogProperties(m_FogProperties);
                        }
                    }
                }
            }
        }

        public override void SetToneMappingProperties(ToneMappingProperties properties)
        {
            if (properties != null)
            {
                m_ToneMappingProperties = properties;

                // Reset current mesh effects with new tone map settings
                foreach (Mesh mesh in m_Meshes)
                {
                    if (mesh.SubMeshes.Count > 0)
                    {
                        foreach (SubMesh subMesh in mesh.SubMeshes)
                        {
                            if (subMesh.RenderEffect != null)
                                subMesh.RenderEffect.SetToneMappingProperties(m_ToneMappingProperties);
                        }
                    }
                }
            }
        }

        public override void SetFXAAProperties(FXAAProperties properties)
        {
            m_FXAAProcessor.SetProperties(properties);
        }

        public override void SetDepthOfFieldProperties(DepthOfFieldProperties properties)
        {
            m_DepthOfFieldPostProcessor.SetProperties(properties);
        }

        public override void SetBloomProperties(BloomProperties properties)
        {
            m_BloomPostProcessor.SetProperties(properties);
        }

        public override void SetFilmGrainProperties(FilmGrainProperties properties)
        {
            m_FilmGrainProcessor.SetProperties(properties);
        }
        #endregion

        #region Property Set / Gets

        // Shadow Properties
        public float ShadowMult { get; set; }

        #endregion
    }
}