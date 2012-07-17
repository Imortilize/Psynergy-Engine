using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using TInput = System.String;
using TOutput = System.String;

using Psynergy.DeferredPipeline;

namespace Psynergy.TerrainPipeline
{
    [ContentProcessor(DisplayName = "Deferred Terrain Model Processor")]
    public class TerrainModelProcessor : DeferredRendererBase
    {
        #region Process
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            return base.Process(input, context);
        }

        #endregion
    }
}
