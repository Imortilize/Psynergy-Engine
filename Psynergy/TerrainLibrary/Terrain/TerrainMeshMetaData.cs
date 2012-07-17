using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SkinnedModel;
using Psynergy.TerrainPipeline;

namespace Psynergy.Graphics.Terrain
{
    public class TerrainMeshMetaData : MeshMetaData.SubMeshMetadata
    {
        private TerrainInfo m_TerrainInfo = null;

        public TerrainMeshMetaData() : base()
        {
        }

        public TerrainInfo TerrainInfo { get { return m_TerrainInfo; } set { m_TerrainInfo = value; } }
    }
}
