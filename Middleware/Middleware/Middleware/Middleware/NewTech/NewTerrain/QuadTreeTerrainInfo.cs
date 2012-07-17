using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Middleware
{
    public class QuadTreeTerrainInfo
    {
        private int m_TopSize = 0;
        private int m_HalfSize = 0;
        private int m_VertexCount = 0;

        public QuadTreeTerrainInfo(Texture2D heightMap)
        {
            if ( heightMap != null )
            {
                m_TopSize = (heightMap.Width - 1);
                m_HalfSize = (int)(m_TopSize * 0.5f);
                m_VertexCount = (heightMap.Width * heightMap.Width);
            }
        }

        #region Property Set / Get
        public int TopSize
        {
            get
            {
                return m_TopSize;
            }
        }

        public int HalfSize
        {
            get
            {
                return m_HalfSize;
            }
        }

        public int VertexCount
        {
            get
            {
                return m_VertexCount;
            }
        }
        #endregion
    }
}
