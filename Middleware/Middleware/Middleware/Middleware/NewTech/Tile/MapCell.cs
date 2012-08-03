using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware
{
    public class MapCell
    {
        private int m_TileID = -1;

        public MapCell(int tileID)
        {
            m_TileID = tileID;
        }

        public int TileID { get { return m_TileID; } set { m_TileID = value; } }
    }
}
