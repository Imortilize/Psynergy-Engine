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
    public class SSAOBlur : PostProcessor<BasicProperties>
    {
        public SSAOBlur(GraphicsDevice graphicsDevice)
            : base(graphicsDevice, new BasicProperties())
        {

        }

         public SSAOBlur(GraphicsDevice graphicsDevice, Effect effect)
            : base(graphicsDevice, effect, new BasicProperties())
        {
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
                // Set textures
                m_GraphicsDevice.Textures[3] = texture;
                m_GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicClamp;

                //Set SSAO parameters
                Effect.Parameters["xBlurDirection"].SetValue(Vector2.One);
                Effect.Parameters["xTargetSize"].SetValue(new Vector2(texture.Width, texture.Height));

                // Draw the initial SSAO
                base.Draw(null);
            }
        }
    }
}
