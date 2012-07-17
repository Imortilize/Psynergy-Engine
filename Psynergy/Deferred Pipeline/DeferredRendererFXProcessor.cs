using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace Psynergy.DeferredPipeline
{
    /// <summary>
    /// This class takes the array of #defines from the material, and insert it into the fx file
    /// </summary>
    [ContentProcessor(DisplayName = "Deferred FX Processor")]
    public class DeferredRendererFXProcessor : EffectProcessor
    {
        public override CompiledEffectContent Process(Microsoft.Xna.Framework.Content.Pipeline.Graphics.EffectContent input, ContentProcessorContext context)
        {
            this.DebugMode = EffectProcessorDebugMode.Optimize;

            if (context.Parameters.ContainsKey("Defines"))
                this.Defines = context.Parameters["Defines"].ToString();

            return base.Process(input, context);
        }
    }
}