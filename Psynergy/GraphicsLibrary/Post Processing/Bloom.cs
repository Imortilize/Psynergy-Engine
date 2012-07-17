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
    public class Bloom : PostProcessor<BloomProperties>
    {
        private GaussianBlur m_BlurPostProcessor = null;
        private MergeBloom m_MergePostProcesser = null;

        public Bloom(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, new BloomProperties())
        {
            m_BlurPostProcessor = new GaussianBlur(graphicsDevice, 3);
            m_MergePostProcesser = new MergeBloom(graphicsDevice);
        }

        public Bloom(GraphicsDevice graphicsDevice, Effect effect, Effect blurEffect, Effect mergeEffect)
            : base(graphicsDevice, effect, new BloomProperties())
        {
            m_BlurPostProcessor = new GaussianBlur(graphicsDevice, blurEffect, 3);
            m_MergePostProcesser = new MergeBloom(graphicsDevice, mergeEffect);
        }

        protected override void CreateRenderCapture()
        {
            m_RenderCapture = new RenderCapture(m_GraphicsDevice);
            m_RenderCapture.AddRenderTarget();
        }

        public override void Reset()
        {
            base.Reset();

            // Reset internal post processors
            m_BlurPostProcessor.Reset();
            m_MergePostProcesser.Reset();
        }

        public override void Draw(Texture2D texture)
        {
            // Depth of field reies on the face we have a graphics device, an effect assigned 
            // a blur post processor and a merge bloom processor in able to effectively work
            if ((m_GraphicsDevice != null) && (Effect != null) && (m_BlurPostProcessor != null) && ( m_MergePostProcesser != null ))
            {
                base.Draw(texture);

                // First blue the image
                m_BlurPostProcessor.Draw(ProcessedImage);

                // Now merge the bloom to the original image
                m_MergePostProcesser.OriginalTexture = texture;          

                // Merge the bloom image and original image together
                m_MergePostProcesser.Draw(m_BlurPostProcessor.ProcessedImage);
            }
        }

        protected override void SetSamplerStates()
        {
            base.SetSamplerStates();

            // Set sampler state
            m_GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        }

        #region Tweakable Values
        public void SetProperties(BloomProperties properties)
        {
            if (properties != null)
            {
                m_Properties = properties;

                if (Effect != null)
                    Effect.Parameters["xThreshold"].SetValue(m_Properties.BloomThreshold);

                m_MergePostProcesser.BloomIntensity = m_Properties.BloomIntensity;
                m_MergePostProcesser.BloomSaturation = m_Properties.BloomSaturation;
                m_MergePostProcesser.OriginalIntensity = m_Properties.OriginalIntensity;
                m_MergePostProcesser.OriginalSaturation = m_Properties.OriginalSaturation;
            }
        }

        public void SetBloomThreshold(float bloomThreshold)
        {
            m_Properties.BloomThreshold = bloomThreshold;

            if (Effect != null)
                Effect.Parameters["xThreshold"].SetValue(m_Properties.BloomThreshold);
        }

        public void SetBloomIntensity(float intensity)
        {
            m_MergePostProcesser.BloomIntensity = intensity;
        }

        public void SetBloomSaturation(float saturation)
        {
            m_MergePostProcesser.BloomSaturation = saturation;
        }

        public void SetOriginalIntensity(float intensity)
        {
            m_MergePostProcesser.OriginalIntensity = intensity;
        }

        public void SetOriginalSaturation(float saturation)
        {
            m_MergePostProcesser.OriginalSaturation = saturation;
        }
        #endregion

        #region Setter Functions
        protected override void SetEffectParameters()
        {
            if (Effect != null)
                Effect.Parameters["xThreshold"].SetValue(m_Properties.BloomThreshold);
        }
        #endregion


        #region Property Set / Gets
        public override Texture2D FinalImage
        {
            get
            {
                return m_MergePostProcesser.ProcessedImage;
            }
        }

        public Effect MergeEffect
        {
            get { return m_MergePostProcesser.Effect; }
            set { m_MergePostProcesser.Effect = value; }
        }

        public Effect BlurEffect
        {
            get { return m_BlurPostProcessor.Effect; }
            set { m_BlurPostProcessor.Effect = value; }
        }
        #endregion
    }
}
