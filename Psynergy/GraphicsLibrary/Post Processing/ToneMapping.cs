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
    public class ToneMapping : PostProcessor<ToneMappingProperties>
    {
        public ToneMapping(GraphicsDevice graphicsDevice) : base(graphicsDevice, new ToneMappingProperties())
        {
        }

        public ToneMapping(GraphicsDevice graphicsDevice, Effect effect)
            : base(graphicsDevice, effect, new ToneMappingProperties())
        {
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Draw(Texture2D texture)
        {
            // Depth of field reies on the face we have a graphics device, an effect assigned 
            // a blur post processor and a merge bloom processor in able to effectively work
            if ((m_GraphicsDevice != null) && (Effect != null))
            {
                base.Draw(texture);
            }
        }

        #region Tweakable Values
        public void SetProperties(ToneMappingProperties properties)
        {
            if (properties != null)
            {
                m_Properties = properties;

                if (Effect != null)
                {
                    //Effect.Parameters["xWhiteLevel"].SetValue(m_Properties.WhiteLevel);
                    //Effect.Parameters["xLuminanceSaturation"].SetValue(m_Properties.LuminanceSaturation);
                    //Effect.Parameters["xOutputWhite"].SetValue(m_Properties.ToneAdjustment);
                }
            }
        }

        public void SetWhiteLevel(float level)
        {
            m_Properties.WhiteLevel = level;

            if (Effect != null)
                Effect.Parameters["xWhiteLevel"].SetValue(m_Properties.WhiteLevel);
        }

        public void SetLuminanceSaturation(float saturation)
        {
            m_Properties.LuminanceSaturation = saturation;

            if (Effect != null)
                Effect.Parameters["xLuminanceSaturation"].SetValue(m_Properties.LuminanceSaturation);
        }

        public void SetToneAdjustment(float factor)
        {
            m_Properties.ToneAdjustment = factor;

            if (Effect != null)
                Effect.Parameters["xOutputWhite"].SetValue(m_Properties.ToneAdjustment);
        }
        #endregion
    }
}
