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
    public class DepthOfField : PostProcessor<DepthOfFieldProperties>
    {
        private Texture2D m_DepthMap = null;
        private Texture2D m_UnBlurred = null;
        private GaussianBlur m_BlurPostProcessor = null;

        public DepthOfField(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, new DepthOfFieldProperties())
        {
            m_BlurPostProcessor = new GaussianBlur(graphicsDevice, 1);
        }

        public DepthOfField(GraphicsDevice graphicsDevice, Effect effect, Effect blurEffect)
            : base(graphicsDevice, effect, new DepthOfFieldProperties())
        {
            m_BlurPostProcessor = new GaussianBlur(graphicsDevice, blurEffect, 1);
        }

        public override void Reset()
        {
            base.Reset();

            // Reset blur post processor
            m_BlurPostProcessor.Reset();
        }

        public override void Draw(Texture2D texture)
        {
            // Depth of field reies on the face we have a graphics device, an effect assigned and 
            // a blur post processor to commit the depth of field against  
            if ((m_GraphicsDevice != null) && (Effect != null) && (m_BlurPostProcessor != null))
            {
                // First blue the image
                m_BlurPostProcessor.Draw(texture);

                // Get the blurred image
                Texture2D blurredImage = m_BlurPostProcessor.ProcessedImage;

                // Set the two textures above to the second and third
                // texture slots
                m_GraphicsDevice.Textures[1] = m_UnBlurred;
                m_GraphicsDevice.Textures[2] = m_DepthMap;

                // Set the samplers for all three textures to PointClamp
                // so we can sample pixel values directly
                m_GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
                m_GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
                m_GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

                base.Draw(blurredImage);
            }
        }

        #region Tweakable Values
        public void SetProperties(DepthOfFieldProperties properties)
        {
            if (properties != null)
            {
                m_Properties = properties;

                if (Effect != null)
                {
                    Effect.Parameters["xBlurStart"].SetValue(m_Properties.BlurStart);
                    Effect.Parameters["xBlurEnd"].SetValue(m_Properties.BlurEnd);
                }

                m_BlurPostProcessor.BlurFactor = m_Properties.BlurAmount;
            }
        }

        public void SetBlurStart(float start)
        {
            m_Properties.BlurStart = start;

            if (Effect != null)
                Effect.Parameters["xBlurStart"].SetValue(m_Properties.BlurStart);
        }

        public void SetBlurEnd(float blurEnd)
        {
            m_Properties.BlurEnd = blurEnd;

            if (Effect != null)
                Effect.Parameters["xBlurEnd"].SetValue(m_Properties.BlurEnd);
        }

        public void SetBlurAmount(float blurAmount)
        {
            m_Properties.BlurAmount = blurAmount;
            m_BlurPostProcessor.BlurFactor = m_Properties.BlurAmount;
        }
        #endregion

        #region Setter Functions
        protected override void SetEffectParameters()
        {
            if (Effect != null)
            {
                Effect.Parameters["xBlurStart"].SetValue(m_Properties.BlurStart);
                Effect.Parameters["xBlurEnd"].SetValue(m_Properties.BlurEnd);
            }
        }
        #endregion

        #region Property Set / Gets
        public Texture2D DepthMap { set { m_DepthMap = value; } }
        public Texture2D UnBlurred { set { m_UnBlurred = value; } }
        public Effect BlurEffect
        {
            get { return m_BlurPostProcessor.Effect; }
            set { m_BlurPostProcessor.Effect = value; }
        }
        #endregion
    }
}
