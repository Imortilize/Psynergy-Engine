using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware
{
    public class MapCell
    {
        private List<int> m_BaseTiles = new List<int>();
        private List<int> m_HeightTiles = new List<int>();
        private List<int> m_TopperTiles = new List<int>();
        private bool m_Walkable = true;

        // Slope Map
        private int m_SlopeMap = -1;

        public MapCell(int tileID)
        {
            // Set base tile id
            TileID = tileID;
        }

        public void AddBaseTile(int tileID)
        {
            m_BaseTiles.Add(tileID);
        }

        public void AddHeightTile(int tileID)
        {
            m_HeightTiles.Add(tileID);
        }

        public void AddTopperTile(int tileID)
        {
            m_TopperTiles.Add(tileID);
        }

        #region Property Set / Gets
        public int TileID 
        { 
            get 
            { 
                return m_BaseTiles.Count > 0 ? m_BaseTiles[0] : 0; 
            } 
            set 
            {
                if (m_BaseTiles.Count > 0)
                    m_BaseTiles[0] = value;
                else
                    AddBaseTile(value);
            } 
        }

        public List<int> BaseTiles { get { return m_BaseTiles; } }
        public List<int> HeightTiles { get { return m_HeightTiles; } }
        public List<int> TopperTiles { get { return m_TopperTiles; } }
        public bool Walkable { get { return m_Walkable; } set { m_Walkable = value; } }
        public int SlopeMap { get { return m_SlopeMap; } set { m_SlopeMap = value; } }
        #endregion
    }
}
