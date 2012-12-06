using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

/* Camera Library */
using Psynergy.Camera;

namespace Psynergy.Graphics
{

    public struct DebugTexture2D
    {
        public DebugTexture2D(Texture2D inTexture, Vector2 inPos, float inScale)
        {
            texture = inTexture;
            position = inPos;
            scale = inScale;
        }

        public Texture2D texture;
        public Vector2 position;
        public float scale;
    };

    public struct DebugBoundingSphere
    {
        public DebugBoundingSphere(Vector3 inCenter, float inRadius)
        {
            center = inCenter;
            radius = inRadius;
        }

        public Vector3 center;
        public float radius;
    };

    public struct DebugPoint
    {
        public DebugPoint(Vector3 inPosition)
        {
            position = inPosition;
            size = 1;
        }

        public DebugPoint(Vector3 inPosition, float inSize)
        {
            position = inPosition;
            size = inSize;
        }

        public Vector3 position;
        public float size;
    };

    public struct DebugLineGroup
    {
        public DebugLineGroup(List<DebugLinePoint> inLinePoints)
        {
            linePoints = inLinePoints;
        }

        public void AddLinePoint(Vector3 point, Color lineColor)
        {
            DebugLinePoint debugLine = new DebugLinePoint(point, lineColor);
            linePoints.Add(debugLine);
        }

        public List<DebugLinePoint> linePoints;
    };

    public struct DebugLinePoint
    {
        public DebugLinePoint(Vector3 inPoint, Color inLineColor)
        {
            linePoint = inPoint;
            lineColor = inLineColor;
        }

        public Vector3 linePoint;
        public Color lineColor;
    };

    public struct DebugVision
    {
        public DebugVision(Vector3 inPos, Vector3 inAxis, Vector3 inMinPos, Vector3 inMaxPos)
        {
            pos = inPos;
            axis = inAxis;
            minPos = inMinPos;
            maxPos = inMaxPos;
        }

        public Vector3 pos;
        public Vector3 axis;
        public Vector3 minPos;
        public Vector3 maxPos;
    };

    public class DebugRender : Singleton<DebugRender>
    {
        public enum RenderType
        {
            Forward = 0,
            Deferred = 1
        };

        // Sprite batch to use
        private SpriteBatch m_SpriteBatch = null;

        // Whether to render debug information or not
        private bool m_Enabled = true;

        // The way to render items
        private RenderType m_RenderType = RenderType.Deferred;

        private RenderTarget2D m_RenderTarget = null;

        // Any output debug 2DTextures
        private List<DebugTexture2D> m_DebugTextures = new List<DebugTexture2D>();

        // Debug sphere variables
        private Model m_BoundingSphereMesh = null;
        private List<DebugBoundingSphere> m_DebugBoundingSpheres = new List<DebugBoundingSphere>();
        private Effect m_DebugSphereEffect = null;

        // Debug point variables
        private Texture2D m_PointTexture = null;
        private Effect m_PointSpriteEffect = null;
        private List<DebugPoint> m_DebugPoints = new List<DebugPoint>();

        // Debug line variables
        private List<DebugLineGroup> m_DebugLines = new List<DebugLineGroup>();
        private BasicEffect m_DebugLineEffect;

        // Debug visions
        private List<DebugVision> m_DebugVisions = new List<DebugVision>();

        // Sprite fonts
        private SpriteFont m_Font = null;

        public DebugRender()
        {
            Viewport viewPort = RenderManager.Instance.GraphicsDevice.Viewport;
            m_RenderTarget = new RenderTarget2D(RenderManager.Instance.GraphicsDevice, viewPort.Width, viewPort.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);

            // get the model to show bounding spheres
            m_BoundingSphereMesh = RenderManager.Instance.LoadModel("Models/FBX/Sphere");

            //m_DebugSphereEffect = RenderManager.Instance.GetEffect("ppgeneric");
            //m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect = m_DebugSphereEffect;

            // Load the point texture
            m_PointTexture = RenderManager.Instance.LoadTexture2D("default");

            // Load the point sprite effect and set initial parameters
            m_PointSpriteEffect = RenderManager.Instance.LoadEffect("pointsprite");
            m_PointSpriteEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
            m_PointSpriteEffect.Parameters["Texture"].SetValue(m_PointTexture);
            m_PointSpriteEffect.Parameters["xPointSpriteSizeWidth"].SetValue(5.0f);
            m_PointSpriteEffect.Parameters["xPointSpriteSizeHeight"].SetValue(5.0f);

            // Debug line effect
            m_DebugLineEffect = new BasicEffect(RenderManager.Instance.GraphicsDevice);
            m_DebugLineEffect.VertexColorEnabled = true;

            // Load test font
            m_Font = RenderManager.Instance.LoadFont("defaultFont");
        }

        public DebugRender(GraphicsDevice graphicsDevice)
        {
            Viewport viewPort = graphicsDevice.Viewport;
            m_RenderTarget = new RenderTarget2D(RenderManager.Instance.GraphicsDevice, viewPort.Width, viewPort.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);

            m_SpriteBatch = new SpriteBatch(graphicsDevice);

            // get the model to show bounding spheres
            m_BoundingSphereMesh = RenderManager.Instance.LoadModel("Models/x/sphere");

            //m_DebugSphereEffect = RenderManager.Instance.GetEffect("ppgeneric");
            //m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect = m_DebugSphereEffect;

            // Load the point texture
            m_PointTexture = RenderManager.Instance.LoadTexture2D("Textures/Others/controlpoint");

            // Load the point sprite effect and set initial parameters
            m_PointSpriteEffect = RenderManager.Instance.LoadEffect("pointsprite");
            m_PointSpriteEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
            m_PointSpriteEffect.Parameters["Texture"].SetValue(m_PointTexture);
            m_PointSpriteEffect.Parameters["xPointSpriteSizeWidth"].SetValue(5.0f);
            m_PointSpriteEffect.Parameters["xPointSpriteSizeHeight"].SetValue(5.0f);

            // Debug line effect
            m_DebugLineEffect = new BasicEffect(graphicsDevice);
            m_DebugLineEffect.VertexColorEnabled = true;

            // Load test font
            m_Font = RenderManager.Instance.LoadFont("defaultFont");
        }

        public void AddTexture2D(Texture2D texture, Vector2 pos, float scale)
        {
            if (m_Enabled)
            {
                DebugTexture2D debugTexture = new DebugTexture2D(texture, pos, scale);

                if (m_RenderType == RenderType.Deferred)
                    m_DebugTextures.Add(debugTexture);
                else
                    RenderTexture(debugTexture);
            }
        }

        public void AddDebugBoundingSphere(Vector3 pos, float radius)
        {
            if (m_Enabled)
            {
                DebugBoundingSphere debugSphere = new DebugBoundingSphere(pos, radius);

                if (m_RenderType == RenderType.Deferred)
                    m_DebugBoundingSpheres.Add(debugSphere);
                else
                    RenderBoundingSphere(debugSphere);
            }
        }

        public void AddDebugPoint(Vector3 pos)
        {
            if (m_Enabled)
            {
                DebugPoint debugPoint = new DebugPoint(pos);

                if (m_RenderType == RenderType.Deferred)
                    m_DebugPoints.Add(debugPoint);
                else
                    RenderPoint(debugPoint);
            }
        }

        public void DrawDebugPoint(Vector3 pos)
        {
            if (m_Enabled)
            {
                DebugPoint debugPoint = new DebugPoint(pos);

                // RenderPoint Straight Away
                RenderPoint(debugPoint);
            }
        }

        public void DrawDebugPoint(Vector3 pos, float size)
        {
            if (m_Enabled)
            {
                DebugPoint debugPoint = new DebugPoint(pos, size);

                // RenderPoint Straight Away
                RenderPoint(debugPoint);
            }
        }

        public void AddDebugPoint(Vector3 pos, float size)
        {
            if (m_Enabled)
            {
                DebugPoint debugPoint = new DebugPoint(pos, size);

                if (m_RenderType == RenderType.Deferred)
                    m_DebugPoints.Add(debugPoint);
                else
                    RenderPoint(debugPoint);
            }
        }

        public void AddNewDebugLineGroup(Vector3 startPos, Color lineColor)
        {
            if (m_Enabled)
            {
                if (m_RenderType == RenderType.Forward)
                {
                    if (m_DebugLines.Count > 0)
                        RenderLineGroup(m_DebugLines[m_DebugLines.Count - 1]);
                }

                m_DebugLines.Add(new DebugLineGroup(new List<DebugLinePoint>()));

                // Add start point
                AddDebugLine(startPos, lineColor);
            }
        }

        public void AddDebugLine(Vector3 point, Color lineColor)
        {
            if (m_Enabled)
            {
                DebugLinePoint debugLine = new DebugLinePoint(point, lineColor);

                if (m_DebugLines.Count > 0)
                    m_DebugLines[m_DebugLines.Count - 1].AddLinePoint(point, lineColor);
            }
        }

        public void AddDebugVision(Vector3 pos, Vector3 axis, Vector3 minPos, Vector3 maxPos)
        {
            if (m_Enabled)
            {
                DebugVision debugVision = new DebugVision(pos, axis, minPos, maxPos);

                // Create new line drawing groups 
                // and add the three vision lines
                AddNewDebugLineGroup(pos, Color.Green);
                AddDebugLine((pos + axis), Color.Green);

                AddNewDebugLineGroup(pos, Color.Green);
                AddDebugLine((pos + minPos), Color.Green);
                AddDebugLine((pos + axis), Color.Green);

                AddNewDebugLineGroup(pos, Color.Green);
                AddDebugLine((pos + maxPos), Color.Green);
                AddDebugLine((pos + axis), Color.Green);

                m_DebugVisions.Add(debugVision);
            }
        }

        public override void Render(GameTime deltaTime)
        {
            if (m_Enabled)
            {
               // RenderManager.Instance.GraphicsDevice.SetRenderTarget(m_RenderTarget);
               // RenderManager.Instance.GraphicsDevice.Clear(Color.Transparent);

                RenderDebugTextures();
                RenderDebugPoints();
                RenderDebugLines();
                RenderDebugBoundingSpheres();
                RenderText();

             //   RenderManager.Instance.GraphicsDevice.SetRenderTarget(null);

                // Draw debug target
                DrawTarget();
            }
        }

        private void DrawTarget()
        {
            if (m_SpriteBatch != null)
            {
                if (m_RenderTarget != null)
                {
                    // Begin the sprite batch
                    m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null);

                    m_SpriteBatch.Draw(m_RenderTarget, Vector2.Zero, Color.White);

                    // End the sprite batch
                    m_SpriteBatch.End();
                }
            }
        }

        private void RenderText()
        {
            if (m_Enabled)
            {
                if (m_SpriteBatch != null)
                {
                    // Now clear the debug texture buffer as these textures will likely change per frame
                    //m_DebugTextures.Clear();
                }
            }
        }

        private void RenderTexture(DebugTexture2D debugTexture)
        {
            if (m_Enabled)
            {
                if (m_SpriteBatch != null)
                {
                    m_SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise); ;
                    m_SpriteBatch.Draw(debugTexture.texture, debugTexture.position, null, Color.White, 0, new Vector2(0, 0), debugTexture.scale, SpriteEffects.None, 1);
                    m_SpriteBatch.End();
                }
            }
        }

        private void RenderDebugTextures()
        {
            if (m_Enabled)
            {
                // Debug texture drawing
                if (m_DebugTextures.Count > 0)
                {
                    if (m_SpriteBatch != null)
                    {
                        m_SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise); ;

                        // Render any commited debug textures
                        foreach (DebugTexture2D debugTexture in m_DebugTextures)
                            m_SpriteBatch.Draw(debugTexture.texture, debugTexture.position, null, Color.White, 0, new Vector2(0, 0), debugTexture.scale, SpriteEffects.None, 1);

                        m_SpriteBatch.End();

                        // Now clear the debug texture buffer as these textures will likely change per frame
                        m_DebugTextures.Clear();
                    }
                }
            }
        }

        private void RenderBoundingSphere(DebugBoundingSphere debugSphere)
        {
            if (m_Enabled)
            {
                Camera3D camera = (RenderManager.Instance.GetActiveCamera() as Camera3D);

                if (camera != null)
                {
                    Matrix world = (Matrix.CreateScale(debugSphere.radius) * Matrix.CreateTranslation(debugSphere.center));
                    Matrix view = camera.View;
                    Matrix projection = camera.Projection;

                    m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect.Parameters["xWorld"].SetValue(world);
                    m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect.Parameters["xView"].SetValue(view);
                    m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect.Parameters["xProjection"].SetValue(projection);
                    m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect.CurrentTechnique = m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect.Techniques["NoTexture"];
                    m_BoundingSphereMesh.Meshes[0].Draw();
                }
            }
        }

        private void RenderDebugBoundingSpheres()
        {
            if (m_Enabled)
            {
                // Debug sphere drawing
                if (m_DebugBoundingSpheres.Count > 0)
                {
                    Camera3D camera = (RenderManager.Instance.GetActiveCamera() as Camera3D);

                    if (camera != null)
                    {
                        RenderManager.Instance.SetBlendState( BlendState.Additive );

                        m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect = m_DebugSphereEffect;
                        m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect.CurrentTechnique = m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect.Techniques["NoTexture"];

                        // Render any commited debug textures
                        foreach (DebugBoundingSphere debugSphere in m_DebugBoundingSpheres)
                        {
                            Matrix world = (Matrix.CreateScale(debugSphere.radius) * Matrix.CreateTranslation(debugSphere.center));
                            Matrix view = camera.View;
                            Matrix projection = camera.Projection;

                            m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect.Parameters["xWorld"].SetValue(world);
                            m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect.Parameters["xView"].SetValue(view);
                            m_BoundingSphereMesh.Meshes[0].MeshParts[0].Effect.Parameters["xProjection"].SetValue(projection);

                            m_BoundingSphereMesh.Meshes[0].Draw();
                        }
                    }

                    // Now clear the debug sphere buffer as these textures will likely change per frame
                    m_DebugBoundingSpheres.Clear();

                    RenderManager.Instance.SetBlendState( BlendState.Opaque );
                }
            }
        }

        private void RenderPoint(DebugPoint debugPoint)
        {
            if (m_Enabled)
            {
                Camera3D camera3D = (RenderManager.Instance.GetActiveCamera() as Camera3D);

                if (camera3D != null)
                {
                    VertexPositionTexture[] point = new VertexPositionTexture[6];
                    int i = 0;

                    // For each debugpoint add the 6 verts required to draw the sprite
                    point[i++] = new VertexPositionTexture(debugPoint.position, new Vector2(1, 1));
                    point[i++] = new VertexPositionTexture(debugPoint.position, new Vector2(0, 0));
                    point[i++] = new VertexPositionTexture(debugPoint.position, new Vector2(1, 0));

                    point[i++] = new VertexPositionTexture(debugPoint.position, new Vector2(1, 1));
                    point[i++] = new VertexPositionTexture(debugPoint.position, new Vector2(0, 1));
                    point[i++] = new VertexPositionTexture(debugPoint.position, new Vector2(0, 0));

                    // Set the effect parameters
                    m_PointSpriteEffect.Parameters["Texture"].SetValue(m_PointTexture);
                    m_PointSpriteEffect.Parameters["xPointSpriteSizeWidth"].SetValue(5.0f);
                    m_PointSpriteEffect.Parameters["xPointSpriteSizeHeight"].SetValue(5.0f);
                    m_PointSpriteEffect.Parameters["xView"].SetValue(camera3D.View);
                    m_PointSpriteEffect.Parameters["xProjection"].SetValue(camera3D.Projection);
                    m_PointSpriteEffect.Parameters["xCamPos"].SetValue(camera3D.transform.Position);
                    m_PointSpriteEffect.Parameters["xCamUp"].SetValue(camera3D.transform.WorldMatrix.Up);

                    foreach (EffectPass pass in m_PointSpriteEffect.CurrentTechnique.Passes)
                    {
                        // Apply the settings to the pass
                        pass.Apply();

                        // Draw the point sprites
                        RenderManager.Instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, point, 0, 2);
                    }
                }
            }
        }

        private void RenderDebugPoints()
        {
            if (m_Enabled)
            {
                // Debug point drawing
                if (m_DebugPoints.Count > 0)
                {
                    // Camera
                    BaseCamera camera = RenderManager.Instance.GetActiveCamera();

                    if ((camera != null) && ((camera.GetType() == typeof(Camera3D)) || camera.GetType().IsSubclassOf(typeof(Camera3D))))
                    {
                        Camera3D camera3D = (camera as Camera3D);

                        VertexPositionTexture[] point = new VertexPositionTexture[m_DebugPoints.Count * 6];
                        int i = 0;

                        // For each debugpoint add the 6 verts required to draw the sprite
                        foreach (DebugPoint debugPoint in m_DebugPoints)
                        {
                            point[i++] = new VertexPositionTexture(debugPoint.position, new Vector2(1, 1));
                            point[i++] = new VertexPositionTexture(debugPoint.position, new Vector2(0, 0));
                            point[i++] = new VertexPositionTexture(debugPoint.position, new Vector2(1, 0));

                            point[i++] = new VertexPositionTexture(debugPoint.position, new Vector2(1, 1));
                            point[i++] = new VertexPositionTexture(debugPoint.position, new Vector2(0, 1));
                            point[i++] = new VertexPositionTexture(debugPoint.position, new Vector2(0, 0));
                        }

                        // Set the effect parameters
                        m_PointSpriteEffect.Parameters["Texture"].SetValue(m_PointTexture);
                        m_PointSpriteEffect.Parameters["xPointSpriteSizeWidth"].SetValue(5.0f);
                        m_PointSpriteEffect.Parameters["xPointSpriteSizeHeight"].SetValue(5.0f);
                        m_PointSpriteEffect.Parameters["xView"].SetValue(camera3D.View);
                        m_PointSpriteEffect.Parameters["xProjection"].SetValue(camera3D.Projection);
                        m_PointSpriteEffect.Parameters["xCamPos"].SetValue(camera3D.transform.Position);
                        m_PointSpriteEffect.Parameters["xCamUp"].SetValue(camera3D.transform.WorldMatrix.Up);

                        foreach (EffectPass pass in m_PointSpriteEffect.CurrentTechnique.Passes)
                        {
                            // Apply the settings to the pass
                            pass.Apply();

                            // Draw the point sprites
                            RenderManager.Instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, point, 0, (m_DebugPoints.Count * 2));
                        }

                        // Clear the debug point list
                        m_DebugPoints.Clear();
                    }
                }
            }
        }

        private void RenderDebugLines()
        {
            if (m_Enabled)
            {
                // Check if there are any debug lines to draw
                if (m_DebugLines.Count > 0)
                {
                    // Camera
                    BaseCamera camera = RenderManager.Instance.GetActiveCamera();

                    if ((camera != null) && ((camera.GetType() == typeof(Camera3D)) || camera.GetType().IsSubclassOf(typeof(Camera3D))))
                    {
                        Camera3D camera3D = (camera as Camera3D);

                        m_DebugLineEffect.View = camera3D.View;
                        m_DebugLineEffect.Projection = camera3D.Projection;
                        m_DebugLineEffect.CurrentTechnique.Passes[0].Apply();

                        // Go through the line groups
                        foreach (DebugLineGroup lineGroup in m_DebugLines)
                        {
                            int lineGroupPointCount = lineGroup.linePoints.Count;

                            if (lineGroupPointCount > 0)
                            {
                                // Buffer to hold the line points
                                VertexPositionColor[] linePoints = new VertexPositionColor[lineGroupPointCount];

                                // Go throuhg the debug lines list and add the points accordingly
                                short[] lineListIndices = new short[lineGroupPointCount];

                                for (int i = 0; i < lineGroupPointCount; i++)
                                {
                                    linePoints[i] = new VertexPositionColor(lineGroup.linePoints[i].linePoint, lineGroup.linePoints[i].lineColor);
                                    lineListIndices[i] = (short)(i);
                                }

                                // Draw the lines
                                if ( (lineGroupPointCount - 1) > 0 )
                                    RenderManager.Instance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, linePoints, 0, lineGroupPointCount, lineListIndices, 0, (lineGroupPointCount - 1));
                            }
                        }

                        // Clear the debug lines buffer
                        m_DebugLines.Clear();
                    }
                }
            }
        }

        public void RenderLineGroup(DebugLineGroup lineGroup)
        {
            // Camera
            BaseCamera camera = RenderManager.Instance.GetActiveCamera();

            if ((camera != null) && ((camera.GetType() == typeof(Camera3D)) || camera.GetType().IsSubclassOf(typeof(Camera3D))))
            {
                Camera3D camera3D = (camera as Camera3D);

                m_DebugLineEffect.View = camera3D.View;
                m_DebugLineEffect.Projection = camera3D.Projection;
                m_DebugLineEffect.CurrentTechnique.Passes[0].Apply();

                // Go through the line groups
                int lineGroupPointCount = lineGroup.linePoints.Count;

                if (lineGroupPointCount > 0)
                {
                    // Buffer to hold the line points
                    VertexPositionColor[] linePoints = new VertexPositionColor[lineGroupPointCount];

                    // Go throuhg the debug lines list and add the points accordingly
                    short[] lineListIndices = new short[lineGroupPointCount];

                    for (int i = 0; i < lineGroupPointCount; i++)
                    {
                        linePoints[i] = new VertexPositionColor(lineGroup.linePoints[i].linePoint, lineGroup.linePoints[i].lineColor);
                        lineListIndices[i] = (short)(i);
                    }

                    // Draw the lines
                    RenderManager.Instance.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, linePoints, 0, lineGroupPointCount, lineListIndices, 0, (lineGroupPointCount - 1));
                }
            }
        }

        public void RenderText(String text, Vector2 position, Color color)
        {
            RenderText(text, position, color, 0.85f, false);
        }

        public void RenderText(String text, Vector2 position, Color color, bool showOutline)
        {
            RenderText(text, position, color, 0.85f, showOutline);
        }

        public void RenderText(String text, Vector2 position, Color color, float size, bool showOutline)
        {
            m_SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            
            if ((m_SpriteBatch != null) && (m_Font != null))
            {
                if (showOutline)
                {
                    m_SpriteBatch.DrawString(m_Font, text, position + new Vector2(1 * size, 1 * size), Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 1.0f);
                    m_SpriteBatch.DrawString(m_Font, text, position + new Vector2(-1 * size, -1 * size), Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 1.0f);
                    m_SpriteBatch.DrawString(m_Font, text, position + new Vector2(-1 * size, 1 * size), Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 1.0f);
                    m_SpriteBatch.DrawString(m_Font, text, position + new Vector2(1 * size, -1 * size), Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 1.0f);
                }

                m_SpriteBatch.DrawString(m_Font, text, position, color, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            }
                
            m_SpriteBatch.End();
        }

        #region Property set/gets
        public bool Enable { get { return m_Enabled; } set { m_Enabled = value; } }
        public RenderType DebugRenderType { get { return m_RenderType; } set { m_RenderType = value; } }
        #endregion
    }
}
