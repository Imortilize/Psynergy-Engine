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
    public class SSAOMerge : PostProcessor<BasicProperties>
    {
        public SSAOMerge(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, new BasicProperties())
        {
            UseBlurredSSAO = true;
        }

        public SSAOMerge(GraphicsDevice graphicsDevice, Effect effect)
            : base(graphicsDevice, effect, new BasicProperties())
        {
            UseBlurredSSAO = true;
        }

        protected override void CreateRenderCapture()
        {
            m_RenderCapture = new RenderCapture(m_GraphicsDevice);
            m_RenderCapture.AddRenderTarget(SurfaceFormat.Color, DepthFormat.None);
        }

        protected override void ClearBuffer()
        {
            m_GraphicsDevice.Clear(Color.White);
        }

        public override void Draw(Texture2D texture)
        {
            // Depth of field reies on the face we have a graphics device, an effect assigned 
            // a blur post processor and a merge bloom processor in able to effectively work
            if ((m_GraphicsDevice != null) && (Effect != null))
            {
                //Set Samplers
                m_GraphicsDevice.Textures[0] = SceneTexture;
                m_GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;

                if (UseBlurredSSAO)
                    m_GraphicsDevice.Textures[1] = texture;
                else
                    m_GraphicsDevice.Textures[1] = SSAOTexture;

                m_GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;

                // Set Effect Parameters
                Effect.Parameters["xHalfPixel"].SetValue(new Vector2(0.5f / texture.Width, 0.5f / texture.Height));

                // Draw the initial SSAO
                base.Draw(null);
            }
        }

        #region Property Set / Gets
        public Texture2D SceneTexture { get; set; }
        public bool UseBlurredSSAO { get; set; }
        public Texture2D SSAOTexture { get; set; }
        #endregion
    }
}
