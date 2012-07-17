using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Psynergy.WaterPipeline
{
    public class WaterInfo
    {
        private float m_TerrainScale = 1;

        // the constructor will initialize all of the member variables.
        public WaterInfo(float terrainScale)
        {
            this.m_TerrainScale = terrainScale;
        }
    }

    /// <summary>
    /// This class will load the TerrainInfo when the game starts. This class needs 
    /// to match the TerrainInfoWriter.
    /// </summary>
    public class WaterInfoReader : ContentTypeReader<WaterInfo>
    {
        protected override WaterInfo Read(ContentReader input, WaterInfo existingInstance)
        {
            float terrainScale = input.ReadSingle();

            // Return new water info
            return new WaterInfo(terrainScale);
        }
    }
}
