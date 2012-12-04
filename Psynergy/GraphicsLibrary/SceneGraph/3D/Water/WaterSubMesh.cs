using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Psynergy.Graphics;
using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class WaterSubMesh : SubMesh
    {
        // Reference to Reflection buffer target texture 
        private RenderTarget2D m_ReflectionTarget = null;
        private RenderTarget2D m_RefractionTarget = null;
        private Texture2D m_OffsetMap = null;
        private Texture2D m_NormalMap = null;

        public WaterSubMesh(Mesh mesh) : base(mesh)
        {
        }

        public WaterSubMesh(Mesh mesh, RenderTarget2D reflectionBuffer, RenderTarget2D refractionBuffer, Texture2D offsetMap, Texture2D normalMap) : base(mesh)
        {
            m_ReflectionTarget = reflectionBuffer;
            m_RefractionTarget = refractionBuffer;
            m_OffsetMap = offsetMap;
            m_NormalMap = normalMap;
        }

        public override void ReflectionRender(GameTime deltaTime, List<AbstractMesh> meshes, Camera3D camera, GraphicsDevice graphicsDevice)
        {
            if ((graphicsDevice != null) && (m_ReflectionTarget != null) && (m_RefractionTarget != null))
            {
                // Now we have access to all the "default" meshes inside the water sub mesh object :)
                if (meshes.Count > 0)
                {
                    // For now hack in a water height
                    const float waterHeight = 1.0f;

                    /* Reimers */
                    Vector3 cameraFinalTarget = (camera.Position + camera.Transform.Forward);

                    // Obtain the camera reflection position
                    Vector3 reflectionCamPosition = camera.Position;
                    reflectionCamPosition.Y = -camera.Position.Y + waterHeight * 2;

                    // Obtain the target of camera B
                    Vector3 reflectionTargetPos = cameraFinalTarget;
                    reflectionTargetPos.Y = -cameraFinalTarget.Y + waterHeight * 2;

                    // Get the up vector of camera B
                    Vector3 invUpVector = Vector3.Cross(camera.Right, (reflectionTargetPos - reflectionCamPosition));

                    // Create the camera B reflection view matrix
                    Matrix reflectionViewMatrix = Matrix.CreateLookAt(reflectionCamPosition, reflectionTargetPos, invUpVector);
                    Matrix reflectionProjectionMatrix = camera.Projection;

                    // Generate a reflection plane
                    Plane reflectionPlane = CreatePlane(camera, (waterHeight - 0.5f), new Vector3(0, -1, 0), reflectionViewMatrix, true);
                    Plane refractionPlane = CreatePlane(camera, ((waterHeight + 4) - 0.5f), new Vector3(0, -1, 0), camera.Transform, false);

                    /*Vector4 testPlane = new Vector4(0, 1, 0, -(waterHeight - 0.5f));
                    reflectionPlane.Normal = new Vector3(testPlane.X, testPlane.Y, testPlane.Z);
                    reflectionPlane.D = testPlane.W;*/

                    // Reflection render
                    graphicsDevice.SetRenderTarget(m_ReflectionTarget);

                    // Clear to black
                    graphicsDevice.Clear(Color.Black);

                    foreach (SubMesh mesh in meshes)
                    {
                        mesh.RenderEffect.SetReflectionClipPlane(reflectionPlane);

                        // Some how render these objects into the reflection target ( need to decide on this still )
                        mesh.ReconstructShading(deltaTime, camera, reflectionViewMatrix, reflectionProjectionMatrix, graphicsDevice);

                        // Set to null now were done with it
                        mesh.RenderEffect.SetReflectionClipPlane(new Plane(Vector4.Zero));
                    }

                    graphicsDevice.SetRenderTarget(null);

                    // Refraction Render
                    graphicsDevice.SetRenderTarget(m_RefractionTarget);

                    // Clear to black
                    graphicsDevice.Clear(Color.Black);

                    foreach (SubMesh mesh in meshes)
                    {
                        mesh.RenderEffect.SetRefractionClipPlane(refractionPlane);

                        // Some how render these objects into the reflection target ( need to decide on this still )
                        mesh.ReconstructShading(deltaTime, camera, camera.Transform, camera.Projection, graphicsDevice);

                        // Set to null now were done with it
                        mesh.RenderEffect.SetRefractionClipPlane(new Plane(Vector4.Zero));
                    }

                    graphicsDevice.SetRenderTarget(null);

                    // Set the water view projection to the water mesh shader
                    EffectParameter waterViewProj = RenderEffect.GetParameter("xWaterViewProjection");
                    if (waterViewProj != null)
                        waterViewProj.SetValue(reflectionViewMatrix * reflectionProjectionMatrix);

                }
            }
        }

        public override void ReconstructShading(GameTime deltaTime, Camera3D camera, Matrix view, Matrix projection, GraphicsDevice graphicsDevice)
        {
            // Set scroll time offset
            RenderEffect.SetScrollTime((float)deltaTime.TotalGameTime.TotalSeconds);

            // Set camera position
            RenderEffect.SetCameraPosition(camera.Position);

            // Render as per normal
            base.ReconstructShading(deltaTime, camera, view, projection, graphicsDevice);
        }

        private Plane CreatePlane(Camera3D camera, float height, Vector3 planeNormalDirection, Matrix currentViewMatrix, bool clipSide)
        {
            planeNormalDirection.Normalize();
            Vector4 planeCoeffs = new Vector4(planeNormalDirection, height);
            if (clipSide)
                planeCoeffs *= -1;

            Matrix worldViewProjection = (currentViewMatrix * camera.Projection);
            Matrix inverseWorldViewProjection = Matrix.Invert(worldViewProjection);
            inverseWorldViewProjection = Matrix.Transpose(inverseWorldViewProjection);

            planeCoeffs = Vector4.Transform(planeCoeffs, inverseWorldViewProjection);

            return new Plane(planeCoeffs);
        }

        #region Effect Setters

        protected override void OnSetEffect(BaseRenderEffect effect)
        {
            if (effect != null)
            {
                effect.SetReflectionBuffer(m_ReflectionTarget);
                effect.SetRefractionBuffer(m_RefractionTarget);
                effect.SetOffsetBuffer(m_OffsetMap);
                effect.SetNormalBuffer(m_NormalMap);
            }
        }
        #endregion
    }
}
