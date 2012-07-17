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
    public class MergeBloom : PostProcessor<BasicProperties>
    {
        private Texture2D m_OriginalTexture = null;
        private float m_BloomIntensity = 1.3f;
        private float m_BloomSaturation = 1.0f;
        private float m_OriginalIntensity = 1.0f;
        private float m_OriginalSaturation = 1.0f;

        public MergeBloom(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, new BasicProperties())
        {
        }

        public MergeBloom(GraphicsDevice graphicsDevice, Effect effect)
            : base(graphicsDevice, effect, new BasicProperties())
        {
        }

        public override void Draw(Texture2D input)
        {
            if (m_RenderCapture != null)
            {
                // Draw gaussian blur
                // Set values for horizontal pass
                if (Effect != null)
                {
                    // Set the original texture into the effect
                    Effect.Parameters["xColorMap"].SetValue(m_OriginalTexture);

                    // Draw the base post processor items for the second time
                    base.Draw(input);
                }
            }
        }

        #region Setter Functions
        protected override void SetEffectParameters()
        {
            if (Effect != null)
            {
                Effect.Parameters["xBloomIntensity"].SetValue(m_BloomIntensity);
                Effect.Parameters["xBloomSaturation"].SetValue(m_BloomSaturation);
                Effect.Parameters["xOriginalIntensity"].SetValue(m_OriginalIntensity);
                Effect.Parameters["xOriginalSaturation"].SetValue(m_OriginalSaturation);
            }
        }
        #endregion

        #region Property Set / Gets
        public Texture2D OriginalTexture { set { m_OriginalTexture = value; } }
        public float BloomIntensity 
        { 
            get { return m_BloomIntensity; } 
            set 
            { 
                m_BloomIntensity = value;

                if (Effect != null)
                    Effect.Parameters["xBloomIntensity"].SetValue(m_BloomIntensity);
            } 
        }
        
        public float BloomSaturation 
        { 
            get { return m_BloomSaturation; } 
            set 
            { 
                m_BloomSaturation = value;

                if (Effect != null)
                    Effect.Parameters["xBloomSaturation"].SetValue(m_BloomSaturation);
            } 
        }

        public float OriginalIntensity 
        { 
            get { return m_OriginalIntensity; } 
            set 
            { 
                m_OriginalIntensity = value;

                if (Effect != null)
                    Effect.Parameters["xOriginalIntensity"].SetValue(m_OriginalIntensity);
            } 
        }

        public float OriginalSaturation 
        { 
            get { return m_OriginalSaturation; } 
            set 
            { 
                m_OriginalSaturation = value;

                if (Effect != null)
                    Effect.Parameters["xOriginalSaturation"].SetValue(m_OriginalSaturation);
            } 
        }
        #endregion

        #region Set / Get
        #endregion
    }
}
