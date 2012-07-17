using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Psynergy.Camera;

namespace Psynergy.Graphics
{
    public class SSAO : PostProcessor<BasicProperties>
    {
        // Internal processors
        private SSAOBlur m_BlurProcessor = null;
        private SSAOMerge m_MergeProcessor = null;

        // Stores depth map
        private Texture2D m_NormalMap = null;
        private Texture2D m_DepthMap = null;

        // Random normal texture
        private Texture2D m_RandomNormals = null;

        // Sample radius
        private float m_SampleRadius = 0.5f;

        // Distance Scale
        private float m_DistanceScale = 100.0f;

        public SSAO(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, new BasicProperties())
        {
            m_BlurProcessor = new SSAOBlur(graphicsDevice, null);
            m_MergeProcessor = new SSAOMerge(graphicsDevice, null);

            m_RandomNormals = RenderManager.Instance.LoadTexture2D("Textures/randomNormal");
        }

        public SSAO(GraphicsDevice graphicsDevice, Effect effect, Effect blurEffect, Effect mergeEffect)
            : base(graphicsDevice, effect, new BasicProperties())
        {
            m_BlurProcessor = new SSAOBlur(graphicsDevice, blurEffect);
            m_MergeProcessor = new SSAOMerge(graphicsDevice, mergeEffect);

            m_RandomNormals = RenderManager.Instance.LoadTexture2D("Textures/randomNormal");
        }

        protected override void CreateRenderCapture()
        {
            m_RenderCapture = new RenderCapture(m_GraphicsDevice);
            m_RenderCapture.AddRenderTarget(SurfaceFormat.Color, DepthFormat.None);
        }

        public override void Draw(Texture2D texture)
        {
            // Depth of field reies on the face we have a graphics device, an effect assigned 
            // a blur post processor and a merge bloom processor in able to effectively work
            if ((m_GraphicsDevice != null) && (Effect != null) && (m_BlurProcessor != null) && (m_MergeProcessor != null))
            {
                Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

                if (camera != null)
                {
                    // Set textures
                    m_GraphicsDevice.Textures[1] = m_NormalMap;
                    m_GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;

                    m_GraphicsDevice.Textures[2] = m_NormalMap;
                    m_GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

                    m_GraphicsDevice.Textures[3] = m_RandomNormals;
                    m_GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicWrap;

                    // Calculate Frustum Corner of the Camera
                    Vector3 cornerFrustum = Vector3.Zero;
                    cornerFrustum.Y = (float)Math.Tan(Math.PI / 3.0 / 2.0) * camera.FarPlane;
                    cornerFrustum.X = (cornerFrustum.Y * camera.AspectRatio);
                    cornerFrustum.Z = camera.FarPlane;

                    // Set SSAO parameters
                    Effect.Parameters["xProjection"].SetValue(camera.Projection);
                    Effect.Parameters["xCornerFrustum"].SetValue(cornerFrustum);
                    Effect.Parameters["xSampleRadius"].SetValue(m_SampleRadius);
                    Effect.Parameters["xDistanceScale"].SetValue(m_DistanceScale);
                    Effect.Parameters["xGBufferSize"].SetValue(m_RenderCapture.GetTargetSize());

                    // Draw the initial SSAO
                    base.Draw(null);

                    // Now merge the bloom to the original image
                    m_MergeProcessor.SceneTexture = texture;
                    m_MergeProcessor.SSAOTexture = ProcessedImage;

                    // First blue the image
                    m_BlurProcessor.Draw(ProcessedImage);

                    // Merge SSAO to scene
                    m_MergeProcessor.Draw(m_BlurProcessor.ProcessedImage);
                }
            }
        }

        protected override void ClearBuffer()
        {
            m_GraphicsDevice.Clear(Color.White);
        }

        #region Property Set / Get
        public override Texture2D FinalImage
        {
            get
            {
                return m_MergeProcessor.ProcessedImage;
            }
        }

        public Texture2D NormalMap { set { m_NormalMap = value; } }
        public Texture2D DepthMap { set { m_DepthMap = value; } }
        public float SampleRadius { get { return m_SampleRadius; } set { m_SampleRadius = value; } }
        public float DistanceScale { get { return m_DistanceScale; } set { m_DistanceScale = value; } }
        #endregion
    }
}
