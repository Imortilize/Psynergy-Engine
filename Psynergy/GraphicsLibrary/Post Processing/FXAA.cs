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
    public class FXAA : PostProcessor<FXAAProperties>
    {
        public FXAA(GraphicsDevice graphicsDevice, Effect effect)
            : base(graphicsDevice, effect, new FXAAProperties())
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
                Vector2 halfPixel = new Vector2(-1f / texture.Width, 1f / texture.Height);
                Vector2 pixelSize = new Vector2(1f / texture.Width, 1f / texture.Height);

                // Neighbour blend
                Effect.Parameters["SCREEN_WIDTH"].SetValue(texture.Width);
                Effect.Parameters["SCREEN_HEIGHT"].SetValue(texture.Height);
                Effect.Parameters["xSceneTexture"].SetValue(texture);

                Matrix projection = Matrix.CreateOrthographicOffCenter(0, m_GraphicsDevice.Viewport.Width, m_GraphicsDevice.Viewport.Height, 0, 0, 1);
                Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
                Effect.Parameters["xMatrixTransform"].SetValue(halfPixelOffset * projection); 

                // Blend the weight
                base.Draw(texture);
            }
        }

        #region Tweakable Values
        public void SetProperties(FXAAProperties properties)
        {
            if (properties != null)
            {
                m_Properties = properties;
            }
        }
        #endregion

        #region Property Set / Gets
        #endregion
    }
}

