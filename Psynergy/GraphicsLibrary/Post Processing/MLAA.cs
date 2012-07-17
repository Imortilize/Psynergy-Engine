using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class MLAA : PostProcessor<BasicProperties>
    {
        private EdgeDetection m_EdgeDetectionProcessor = null;
        //private MLAABlendWeights m_BlendWeightsProcessor = null;

        // Textures used
        private Texture2D m_DepthMap = null;
        private Texture2D m_AreaMap = null;

        public MLAA(GraphicsDevice graphicsDevice, Effect effect, Effect edgeDetectionEffect)
            : base(graphicsDevice, effect, new BasicProperties())
        {
            m_EdgeDetectionProcessor = new EdgeDetection(graphicsDevice, edgeDetectionEffect);
            m_AreaMap = RenderManager.Instance.LoadTexture2D("Shaders/Textures/MLAA/AreaMap32");
        }

        protected override void CreateRenderCapture()
        {
            m_RenderCapture = new MultiPassRenderCapture(m_GraphicsDevice);
            m_RenderCapture.AddRenderTarget(SurfaceFormat.Color, DepthFormat.None);
            m_RenderCapture.AddRenderTarget(SurfaceFormat.Color, DepthFormat.None);
        }

        public override void Reset()
        {
            base.Reset();

            m_EdgeDetectionProcessor.Reset();
        }

        protected override void ClearBuffer()
        {
            MultiPassRenderCapture renderCapture = (m_RenderCapture as MultiPassRenderCapture);

            if (renderCapture.PassIndex == 1)
                m_GraphicsDevice.Clear(Color.Transparent);
            else
                m_GraphicsDevice.Clear(Color.Black);
        }

        public override void Draw(Texture2D texture)
        {
            // Depth of field reies on the face we have a graphics device, an effect assigned 
            // a blur post processor and a merge bloom processor in able to effectively work
            if ((m_GraphicsDevice != null) && (Effect != null))
            {
                m_GraphicsDevice.BlendState = BlendState.Opaque;
                m_GraphicsDevice.DepthStencilState = DepthStencilState.None;
                m_GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

                Vector2 halfPixel = new Vector2(-1f / texture.Width, 1f / texture.Height);
                Vector2 pixelSize = new Vector2(1f / texture.Width, 1f / texture.Height);

                //base.Draw(texture);
                // Set Effect Parameters
                m_EdgeDetectionProcessor.Effect.Parameters["xHalfPixel"].SetValue(halfPixel);
                m_EdgeDetectionProcessor.Effect.Parameters["xPixelSize"].SetValue(pixelSize);
                m_EdgeDetectionProcessor.Effect.Parameters["xSceneTexture"].SetValue(texture);
                m_EdgeDetectionProcessor.Effect.Parameters["xDepthTexture"].SetValue(m_DepthMap);
                m_EdgeDetectionProcessor.DepthMap = m_DepthMap;

                m_GraphicsDevice.SamplerStates[12] = SamplerState.AnisotropicClamp;
                m_GraphicsDevice.SamplerStates[13] = SamplerState.PointClamp;

                m_EdgeDetectionProcessor.Draw(null);

                // Draw Blend Weights
                /*m_BlendWeightsProcessor.Effect.Parameters["xHalfPixel"].SetValue(halfPixel);
                m_BlendWeightsProcessor.Effect.Parameters["xPixelSize"].SetValue(pixelSize);
                m_BlendWeightsProcessor.Effect.Parameters["xEdgeTexture"].SetValue(m_EdgeDetectionProcessor.FinalImage);
                m_BlendWeightsProcessor.Draw(null);*/

                // Neighbour blend
                Effect.Parameters["xHalfPixel"].SetValue(halfPixel);
                Effect.Parameters["xPixelSize"].SetValue(pixelSize);
                Effect.Parameters["xSceneTexture"].SetValue(texture);
                Effect.Parameters["xAreaTexture"].SetValue(m_AreaMap);
                Effect.Parameters["xEdgeTexture"].SetValue(m_EdgeDetectionProcessor.FinalImage);

                Effect.CurrentTechnique = Effect.Techniques["BlendingWeight"];

                m_GraphicsDevice.Clear(Color.Transparent);
                m_GraphicsDevice.SamplerStates[14] = SamplerState.AnisotropicClamp;
                m_GraphicsDevice.SamplerStates[15] = SamplerState.AnisotropicWrap;

                // Blend the weight
                base.Draw(null);

                Effect.Parameters["xBlendedWeightsTexture"].SetValue(GetProcessedImage(0));
                Effect.CurrentTechnique = Effect.Techniques["NeighborhoodBlending"];

                m_GraphicsDevice.SamplerStates[12] = SamplerState.AnisotropicClamp;
                m_GraphicsDevice.SamplerStates[13] = SamplerState.AnisotropicClamp;

                // Blend the neighbours
                base.Draw(null);

                m_GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
        }

        #region Property Set / Gets
        public Texture2D DepthMap { set { m_DepthMap = value; } }
        #endregion
    }
}
