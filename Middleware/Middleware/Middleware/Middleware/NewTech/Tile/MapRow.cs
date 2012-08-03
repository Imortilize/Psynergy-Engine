using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Middleware
{
    public class MapRow
    {
        private List<MapCell> m_Columns = new List<MapCell>();

        public MapRow()
        {
        }

        public List<MapCell> Columns { get { return m_Columns; } }
    }
}
