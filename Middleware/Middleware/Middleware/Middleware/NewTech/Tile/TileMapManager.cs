using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Psynergy;

namespace Middleware
{
    public class TileMapManager : Singleton<TileMapManager>
    {
        private List<TileMap> m_TileMaps = new List<TileMap>();

        public TileMapManager() : base()
        {
        }

        public void AddTileMap( TileMap tileMap )
        {
            Debug.Assert(tileMap != null);
            if ( tileMap != null )
            {
                bool containsMap = m_TileMaps.Contains(tileMap);
                Debug.Assert(!containsMap);

                if (!containsMap)
                    m_TileMaps.Add(tileMap);
            }
        }

        public TileMap GetCurrentTileMap()
        {
            TileMap toRet = null;

            if (m_TileMaps.Count > 0)
                toRet = m_TileMaps[0];

            return toRet;
        }

        public List<TileMap> TileMaps { get { return m_TileMaps; } }
    }
}
