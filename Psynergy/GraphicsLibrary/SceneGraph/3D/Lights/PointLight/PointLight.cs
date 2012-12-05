using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Psynergy;
using Psynergy.Input;
using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class PointLight : Light, IRegister<PointLight>
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterFloat("radius", "Radius");
            factory.RegisterFloat("intensity", "Intensity");

            base.ClassProperties(factory);
        }
        #endregion

        #region Volume Models
        private Model m_SphereModel = null;
        #endregion
        
        public BoundingSphere m_BoundingSphere;
        private float m_Radius = 0.0f;

        #region Depth State Holders
        private DepthStencilState m_CcwDepthState;
        private DepthStencilState m_CwDepthState;
        #endregion

        public PointLight()
        {
            DiffuseColor = new Vector3(1, 1, 1);
            Intensity = 1f;
            Radius = 10000;
            BoundingSphere = new BoundingSphere(Position, Radius);
        }

        public override void Initialise()
        {
            base.Initialise();

            m_Effect = RenderManager.Instance.GetEffect("PointLight").Clone();
            m_Effect.Parameters["xLightColor"].SetValue((DiffuseColor / 255));
            m_Effect.Parameters["xLightRadius"].SetValue(Radius);
            m_Effect.Parameters["xLightIntensity"].SetValue(Intensity);

            Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

            if ( camera != null )
               m_Effect.Parameters["xFarPlane"].SetValue(camera.FarPlane);
        }

        public override void Load()
        {
            base.Load();

            // Load volumetric models
            m_SphereModel = RenderManager.Instance.LoadModel("Models/X/sphere");
        }

        protected override void UpdateInput(GameTime gameTime)
        {
            if (m_InputAllowed)
            {
                if (InputHandle.GetKey(Keys.D))
                    Radius += 10f;

                if (InputHandle.GetKey(Keys.A))
                    Radius -= 10f;

                if (InputHandle.GetKey(Keys.Right))
                    Position += new Vector3(0.5f, 0, 0);

                if (InputHandle.GetKey(Keys.Left))
                    Position -= new Vector3(0.5f, 0, 0);

                if (InputHandle.GetKey(Keys.Up))
                    Position += new Vector3(0, 0.5f, 0);

                if (InputHandle.GetKey(Keys.Down))
                    Position -= new Vector3(0, 0.5f, 0);
            }
        }

        public override void SetEffectParameters(Effect effect)
        {
            base.SetEffectParameters(effect);

            if (effect.Parameters["xLightPosition"] != null)
                effect.Parameters["xLightPosition"].SetValue(Position);

            if (effect.Parameters["xLightAttenuation"] != null)
                effect.Parameters["xLightAttenuation"].SetValue(Radius);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);
        }

        public override void Draw(GameTime deltaTime)
        {
            base.Draw(deltaTime);

            GraphicsDevice graphicsDevice = RenderManager.Instance.GraphicsDevice;

            if (graphicsDevice != null)
            {
                if ((m_Effect != null) && (m_SphereModel != null))
                {
                    Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

                    // Compute the light world matrix
                    // and scale according to light radius, and translate it to light position
                    Matrix sphereWorldMatrix = Matrix.CreateScale(Radius);
                    sphereWorldMatrix.Translation = BoundingSphere.Center;

                    m_Effect.Parameters["xWorld"].SetValue(sphereWorldMatrix);
                    m_Effect.Parameters["xLightPosition"].SetValue(Position);

                    if (camera != null)
                    {
                        m_Effect.Parameters["xView"].SetValue(camera.Transform);
                        m_Effect.Parameters["xProjection"].SetValue(camera.Projection);
                        m_Effect.Parameters["xCameraPosition"].SetValue(camera.Position);
                        m_Effect.Parameters["xInvertViewProjection"].SetValue(Matrix.Invert(camera.ViewProjection));

                        // Calculate the distance between the camera and light center
                        float cameraToCenter = Vector3.Distance(camera.Position, Position);

                        Vector3 test = (camera.Transform.Forward * camera.FarPlane);

                        //if (camera.Frustum.Far.Intersects(BoundingSphere) == PlaneIntersectionType.Intersecting)
                        //graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                        if (cameraToCenter < Radius)
                        {
                            //if we are inside the light volume, draw the sphere's inside face
                            graphicsDevice.RasterizerState = RasterizerState.CullClockwise;

                            // Inside bounding object
                            m_Effect.Parameters["xInsideBoundingObject"].SetValue(true);
                        }
                        else
                        {
                            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                            // Not inside bounding object
                            m_Effect.Parameters["xInsideBoundingObject"].SetValue(false);
                        }
                    }

                    // No depth stencil
                    graphicsDevice.DepthStencilState = DepthStencilState.None;

                    // Apply the effect
                    m_Effect.Techniques[0].Passes[0].Apply();
                }

                // Draw the sphere
                foreach (ModelMesh mesh in m_SphereModel.Meshes)
                {
                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    {
                        graphicsDevice.Indices = meshPart.IndexBuffer;
                        graphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);

                        graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
                    }
                }

                graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                graphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
        }

        #region Property Set / Gets
        public BoundingSphere BoundingSphere { get { return m_BoundingSphere; } set { m_BoundingSphere = value; } }
        public float Radius
        {
            get { return m_Radius; }
            set
            {
                m_Radius = value;
                m_BoundingSphere.Radius = value;
            }
        }

        public override Vector3 Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;
                
                // Set bounding sphere center
                m_BoundingSphere.Center = Position;
            }
        }

        public float Intensity { get; set; }
        #endregion
    }
}
