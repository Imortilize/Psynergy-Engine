using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Psynergy;
using Psynergy.Camera;
using Psynergy.AI;
using Psynergy.Input;

namespace Psynergy.Graphics
{
    public enum RendererEngineType
    {
        None = 0,
        Deferred = 1,
        Sprite = 2
    };

    public class RenderManager : Singleton<RenderManager>
    {
        public enum GraphicsSettings
        {
            Low = 0,
            High = 1
        };

        // Renderer type
        private RendererEngineType m_RendererEngineType = RendererEngineType.None;
        protected ContentManager m_ContentManager = null;
        protected GraphicsDeviceManager m_Graphics = null;
        protected SpriteBatch m_SpriteBatch = null;

        // State manager
        protected StateMachine<GameObject> m_StateManager = null;

        // Renderer to be used
        private PsynergyRenderer m_Renderer = null;

        // Effects buffer
        protected SortedList<String, Effect> m_Effects = new SortedList<String, Effect>();

        // Rendergroups Buffer
        protected RenderGroupResource m_RenderGroupResource = null;
        protected Dictionary<String, RenderGroup> m_RenderGroups = new Dictionary<String, RenderGroup>();

        // Debug render ( to show any debug information )
        protected DebugRender m_DebugRender = null;

        // Default resolution
        private Vector2 m_BaseResolution = new Vector2(1280, 720);

        // Graphics settings
        private GraphicsSettings m_GraphicsSettings = GraphicsSettings.Low;

        // Particle system
        private ParticleEngine m_ParticleManager = null;

        #region Tree Manager
        private TreeManager m_TreeManager = null;
        #endregion

        public RenderManager()
        {
        }

        public RenderManager(ContentManager contentManager, GraphicsDeviceManager graphicsDevice, StateMachine<GameObject> stateManager)
        {
            m_ContentManager = contentManager;
            m_Graphics = graphicsDevice;
            m_StateManager = stateManager;

            // Create particle manager
            m_ParticleManager = new ParticleEngine(contentManager, graphicsDevice);
            
            // Create LTree renderer ( for now here but probably decoupled later ) TODO:
            m_TreeManager = new TreeManager();
        }

        public override void Initialise()
        {
            if ( m_Graphics != null )
            {
                if ( m_Graphics.GraphicsDevice != null )
                {
                    // Create renderer
                    CreateRenderer(m_ContentManager, m_Graphics);

                    // Initialise the renderer
                    if (m_Renderer != null)
                        m_Renderer.Initialise();
                }
            }

            // Initialise the tree manager
            if (m_TreeManager != null)
                m_TreeManager.Initialise();

            // initialise particle manager
            if (m_ParticleManager != null)
                m_ParticleManager.Initialise();

            // Initialise the state manager
            if (m_StateManager != null)
                m_StateManager.Initialise();
        }

        private void CreateRenderer(ContentManager contentManager, GraphicsDeviceManager graphicsDevice)
        {
            switch (m_RendererEngineType)
            {
                case RendererEngineType.None:
                    {
                        // Create basic renderer ( is a stub renderer so won't render anything )
                        m_Renderer = new PsynergyRenderer(contentManager, graphicsDevice);
                    }
                    break;
                case RendererEngineType.Deferred:
                    {
                        // 3D deferred rendering engine
                        m_Renderer = new DeferredRenderer(contentManager, graphicsDevice);
                    }
                    break;
                case RendererEngineType.Sprite:
                    {
                        // 2D sprite renderer
                        m_Renderer = new SpriteRenderer(contentManager, graphicsDevice);
                    }
                    break;
            }
        }

        public override void Reset()
        {
            base.Reset();

            if (m_RendererEngineType == RendererEngineType.Deferred)
            {
                // Reset effect targets
                if (m_Renderer != null)
                    m_Renderer.Reset();
            }
        }

        public void ResetParticles()
        {
            // initialise particle manager
            if (m_ParticleManager != null)
                m_ParticleManager.Reset();
        }

        public override void Load()
        {
            // Load effect files
            LoadEffect("Shaders/Shadows/ShadowDepthMap");
            LoadEffect("Shaders/Others/pointsprite");
            LoadEffect("Shaders/CubeMap/cubemap");
            LoadEffect("Shaders/Others/Billboard");

            // Lights
            LoadEffect("Shaders/Deferred Rendering/DirectionalLight");
            LoadEffect("Shaders/Deferred Rendering/PointLight");

            // Post processing effects
            LoadEffect("Shaders/Post Processing/DOFPostProcessor");
            LoadEffect("Shaders/Post Processing/BWPostProcessor");
            LoadEffect("Shaders/Post Processing/GaussianBlurPostProcessor");
            LoadEffect("Shaders/Post Processing/BloomPostProcessor");
            LoadEffect("Shaders/Post Processing/MergeBloomPostProcessor");
            LoadEffect("Shaders/Post Processing/EdgeDetectionPostProcessor");
            LoadEffect("Shaders/Post Processing/MLAAPostProcessor");
            LoadEffect("Shaders/Post Processing/FilmMappingPostProcessor");
            LoadEffect("Shaders/Post Processing/SSAO");
            LoadEffect("Shaders/Post Processing/SSAOBlur");
            LoadEffect("Shaders/Post Processing/SSAOMerge");
            LoadEffect("Shaders/Post Processing/FXAA"); 

            #region Deferred Rendering
            LoadEffect("Shaders/Deferred Rendering/RenderGBuffer");
            LoadEffect("Shaders/Deferred Rendering/WaterGBuffer");
            #endregion

            // Load render groups
            m_RenderGroupResource = new RenderGroupResource("Resources/RenderGroups.xml");

            // Load the resource
            if (m_RenderGroupResource != null)
                m_RenderGroupResource.Load();

            if (m_Renderer != null)
                m_Renderer.Load();

            // load the tree manager
            if (m_TreeManager != null)
                m_TreeManager.Load();

            // Load particle related items
            if (m_ParticleManager != null)
                m_ParticleManager.Load();

            // load the state manager
            if (m_StateManager != null)
                m_StateManager.Load();

            // Create the debug renderer
            m_DebugRender = new DebugRender(GraphicsDevice);
        }

        public Texture2D LoadTexture2D( String assetName )
        {
            Texture2D toRet = null;

            // Check a texture name exists
            if (assetName != null)
            {
                if (assetName == "")
                    assetName = "default";

                toRet = m_ContentManager.Load<Texture2D>(assetName);

                Debug.Assert(toRet != null, "Texture file '" + assetName + "' not found / loaded on a sprite node!");
            }

            return toRet;
        }

        public SpriteFont LoadFont(String assetName)
        {
            SpriteFont toRet = null;

            // Check a texture name exists
            if (assetName != null)
            {
                if (assetName == "")
                    assetName = "default";

                toRet = m_ContentManager.Load<SpriteFont>("Fonts/" + assetName);

                Debug.Assert(toRet != null, "Texture file '" + assetName + "' not found / loaded on a sprite node!");
            }

            return toRet;
        }

        public TextureCube LoadTextureCube(String assetName)
        {
            TextureCube toRet = null;

            // Check a texture name exists
            if (assetName != null)
            {
                if (assetName == "")
                    assetName = "default";

                toRet = m_ContentManager.Load<TextureCube>(assetName);

                Debug.Assert(toRet != null, "Texture file '" + assetName + "' not found / loaded on a sprite node!");
            }

            return toRet;
        }

        public Model LoadModel(String assetName)
        {
            Model toRet = null;

            // Check a texture name exists
            if (assetName != null)
            {
                if (assetName == "")
                    assetName = "default";

                toRet = m_ContentManager.Load<Model>(assetName);

                Debug.Assert(toRet != null, "Texture file '" + assetName + "' not found / loaded on a sprite node!");
            }

            return toRet;
        }

        public Effect LoadEffect(String assetName)
        {
            return LoadEffect(assetName, false);
        }

        public Effect LoadEffect(String assetName, bool clone)
        {
            Effect toRet = null;

            // Check if the effect has already been loaded or not
            if (m_Effects != null)
                m_Effects.TryGetValue(assetName, out toRet);

            if (toRet == null)
            {
                toRet = m_ContentManager.Load<Effect>(assetName);

                if (m_Effects != null)
                {
                    int folderIndex = assetName.LastIndexOf("/");
                    String effectName = assetName.Substring( folderIndex + 1);

                    m_Effects.Add(effectName, toRet);
                }
            }

            if (clone)
                toRet = toRet.Clone();

            // Set effect name
            if (toRet != null)
                toRet.Name = assetName;

            return toRet;
        }

        public Effect GetEffect(String effectName)
        {
            Effect toRet = null;

            Debug.Assert(m_Effects != null, "Effects buffer cannot be null if trying to set an effect!");

            // Check if the effect has already been loaded or not
            if (m_Effects != null)
            {
                bool success = m_Effects.TryGetValue(effectName, out toRet);

                // If the effect wasn't found, try to load it
                if (!success)
                    toRet = LoadEffect(effectName);

                Debug.Assert(toRet != null, "Effect " + effectName + " was not found in the effect buffer and couldn't be loaded!");
            }

            return toRet;
        }

        public override void Update(GameTime deltaTime)
        {
            if (!InputManager.Instance.PauseRender)
            {
                if (m_Renderer != null)
                    m_Renderer.Update(deltaTime);
            }

            // Update the current state 
            if (m_StateManager != null)
                m_StateManager.Update(deltaTime);

            if (!InputManager.Instance.PauseRender)
            {
                // Update particle related items
                if (m_ParticleManager != null)
                    m_ParticleManager.Update(deltaTime);
            }
        }

        public override void Render(GameTime deltaTime)
        {
            if (m_Renderer != null)
            {
                // Begin renderer
                m_Renderer.Begin();

                // Draw renderer
                m_Renderer.Draw(deltaTime);

                // Render the current State for any post graphics rendering
                RenderState(deltaTime);

                // Particles should be here but arn't at the moment
                RenderParticles(deltaTime);
 
                // End renderer
                m_Renderer.End();
            }
        }

        private void RenderState(GameTime deltaTime)
        {
            // Render the current state
            if (m_StateManager != null)
                m_StateManager.Render(deltaTime);
        }

        public void RenderParticles(GameTime deltaTime)
        {
            if (m_ParticleManager != null)
                m_ParticleManager.Render(deltaTime);
        }

        public void RenderDebug(GameTime deltaTime)
        {
            if (m_DebugRender != null)
                m_DebugRender.Render(deltaTime);
        }

        // Render group functions
        public void AddRenderGroup(RenderGroup renderGroup)
        {
            RenderGroup check = null;

            // Check that this render group doesn't already exist
            m_RenderGroups.TryGetValue(renderGroup.Name, out check);

            Debug.Assert(check == null, "Can't add a render group that already exists!");

            if (check == null)
                m_RenderGroups.Add(renderGroup.Name, renderGroup);
        }

        public RenderGroup FindRenderGroup(String renderGroupName)
        {
            RenderGroup toRet = null;

            // Try to obtain the rendergroup
            m_RenderGroups.TryGetValue(renderGroupName, out toRet);

            return toRet;
        }

        public override void UnLoad()
        {
            // unload the state manager
            if (m_StateManager != null)
                m_StateManager.UnLoad();
        }

        public BaseCamera GetActiveCamera()
        {
            return CameraManager.Instance.ActiveCamera;
        }

        public Vector2 Project3DTo2D(Vector3 pos, Matrix mat)
        {
            Vector2 toRet = Vector2.Zero;

            BaseCamera camera = CameraManager.Instance.ActiveCamera;

            if (camera != null)
            {
                Type cameraType = camera.GetType();

                if ((cameraType == typeof(Camera3D)) || (cameraType.IsSubclassOf(typeof(Camera3D))))
                {
                    Camera3D camera3D = (camera as Camera3D);

                    Matrix view = camera3D.View;
                    Matrix proj = camera3D.Projection;

                    Vector3 projectedPosition = GraphicsDevice.Viewport.Project(pos, proj, view, Matrix.Identity);
                    
                    // Now turn projected position into 2D
                    toRet = new Vector2(projectedPosition.X, projectedPosition.Y);
                }
            }

            return toRet;
        }

        public void SetBackBufferTarget()
        {
            if (m_Renderer != null)
                m_Renderer.SetBackBufferTarget();
        }

        public void SetBlendState(BlendState blendState)
        {
            if (m_Renderer != null)
                m_Renderer.BlendState = blendState;
        }

        public void SetDepthStencilState(DepthStencilState stencilState)
        {
            if (m_Renderer != null)
                m_Renderer.DepthStencilState = stencilState;
        }

        public void SetCullMode(CullMode cullMode)
        {
            if (m_Renderer != null)
                m_Renderer.CullMode = cullMode;
        }

        #region Graphics Settings
        public void SetEngineType(RendererEngineType engineType)
        {
            m_RendererEngineType = engineType;
        }

        public void ToggleGraphicsSettings()
        {
            bool useShadows = false;
            int samplingCount = 0;
            bool usePostProcessing = false;

            switch (m_GraphicsSettings)
            {
                case GraphicsSettings.Low:
                    {
                        useShadows = true;
                        samplingCount = 8;
                        usePostProcessing = true;

                        m_GraphicsSettings = GraphicsSettings.High;
                    }
                    break;
                case GraphicsSettings.High:
                    {
                        useShadows = false;
                        samplingCount = 0;
                        usePostProcessing = false;

                        m_GraphicsSettings = GraphicsSettings.Low;
                    }
                    break;
            }

            // Set shadow settings
            UseShadows(useShadows);

            // Set anti-aliasing settings
            SetAntiAliasing(samplingCount);

            // Use Post processing
            UsePostProcesing(usePostProcessing);
        }

        public void UseShadows(bool useShadows)
        {
            if (m_Renderer != null)
                m_Renderer.UseShadows(useShadows);
        }

        public void SetAntiAliasing(int samples)
        {
            if (GraphicsDevice != null)
            {
                GraphicsDevice.PresentationParameters.MultiSampleCount = samples;
                GraphicsDeviceManager.ApplyChanges();

                if (m_Renderer != null)
                    m_Renderer.ResetAntiAliasing();
            }
        }

        public void SetFogProperties(FogProperties properties)
        {
            if (m_Renderer != null)
                m_Renderer.SetFogProperties(properties);
        }

        public void UsePostProcesing(bool use)
        {
            if (m_Renderer != null)
                m_Renderer.UsePostProcessing(use);
        }

        public void SetToneMappingProperties(ToneMappingProperties properties)
        {
            if (m_Renderer != null)
                m_Renderer.SetToneMappingProperties(properties);
        }

        public void SetFXAAProperties(FXAAProperties properties)
        {
            if (m_Renderer != null)
                m_Renderer.SetFXAAProperties(properties);
        }

        public void SetDepthOfFieldProperties(DepthOfFieldProperties properties)
        {
            if (m_Renderer != null)
                m_Renderer.SetDepthOfFieldProperties(properties);
        }

        public void SetBloomProperties(BloomProperties properties)
        {
            if (m_Renderer != null)
                m_Renderer.SetBloomProperties(properties);
        }

        public void SetFilmGrainProperties(FilmGrainProperties properties)
        {
            if (m_Renderer != null)
                m_Renderer.SetFilmGrainProperties(properties);
        }
        #endregion

        #region Particle System Settings
        public void SetParticleEngine(ParticleEngineType engineType)
        {
            if (m_ParticleManager != null)
                m_ParticleManager.SetParticleEngine(engineType);
        }
        #endregion

        public RendererEngineType EngineType { get { return m_RendererEngineType; } }
        public ContentManager ContentManager { get { return m_ContentManager; } set { m_ContentManager = value; } }
        public GraphicsDeviceManager GraphicsDeviceManager { get { return m_Graphics; } set { m_Graphics = value; } }
        public GraphicsDevice GraphicsDevice { get { return m_Graphics.GraphicsDevice; } }
        public SpriteBatch SpriteBatch 
        { 
            get 
            { 
                return m_SpriteBatch; 
            } 
            set 
            { 
                m_SpriteBatch = value;

                if (m_Renderer != null)
                    m_Renderer.SpriteBatch = value;
            } 
        }
        public StateMachine<GameObject> GameStateManager { get { return m_StateManager; } }
        public PsynergyRenderer ActiveRenderer { get { return m_Renderer; } }
        public Vector2 BaseResolution { get { return m_BaseResolution; } set { m_BaseResolution = value; } }
    }
}
