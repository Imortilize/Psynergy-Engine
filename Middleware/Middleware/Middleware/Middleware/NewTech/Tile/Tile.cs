using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Psynergy;
using Psynergy.Graphics;

namespace Middleware
{
    public class Tile
    {
        public Tile()
        {
        }

        public Rectangle GetSourceRectangle(int tileIndex)
        {
            return new Rectangle((tileIndex * 32), 0, 32, 32);
        }
    }
}
