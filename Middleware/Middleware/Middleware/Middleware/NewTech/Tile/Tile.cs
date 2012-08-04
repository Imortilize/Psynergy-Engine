using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Psynergy;
using Psynergy.Graphics;

namespace Middleware
{
    public class Tile
    {
        private int m_Width = 48;
        private int m_Height = 48;

        public Tile()
        {
        }

        public Rectangle GetSourceRectangle(Texture2D texture, int tileIndex)
        {
            Rectangle toRet = Rectangle.Empty;

            if (texture != null)
            {
                int tileY = tileIndex / (texture.Width / m_Width);
                int tileX = tileIndex % (texture.Width / m_Width);

                toRet = new Rectangle(tileX * m_Width, tileY * m_Height, m_Width, m_Height);
            }

            return toRet;
        }
    }
}
