using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SkinnedModel;
using Psynergy.WaterPipeline;

namespace Psynergy.Graphics
{
    public class WaterMeshMetaData : MeshMetaData.SubMeshMetadata
    {
        private WaterInfo m_WaterInfo = null;

        public WaterMeshMetaData() : base()
        {
        }

        public WaterInfo WaterInfo { get { return m_WaterInfo; } set { m_WaterInfo = value; } }
    }
}
