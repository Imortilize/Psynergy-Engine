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

namespace Psynergy.WaterPipeline
{ 
    [ContentProcessor(DisplayName = "Deferred Water Model Processor")]
    [Browsable(false)]
    public class WaterModelProcessor : ModelProcessor
    {
        #region Properties
        [Browsable(false)]
        public override bool GenerateTangentFrames
        {
            get { return true; }
            set { }
        }
        #endregion

        #region Acceptable Vertex Channels
        static IList<string> m_AcceptableVertexChannelNames = new string[] {
                VertexChannelNames.TextureCoordinate(0),
                VertexChannelNames.Normal(0),
                VertexChannelNames.Binormal(0),
                VertexChannelNames.Tangent(0),
                VertexChannelNames.Weights(0)
            };
        #endregion

        #region Process
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            context.Logger.LogMessage("[DOM] - WATER MODEL PROCESSOR YER!!!");

            // Chain to the base ModelProcessor class so it can convert the model data.
            ModelContent model = base.Process(input, context);

            // Return model data
            return model;
        }

        protected override void ProcessVertexChannel(GeometryContent geometry, int vertexChannelIndex, ContentProcessorContext context)
        {
            String vertexChannelName = geometry.Vertices.Channels[vertexChannelIndex].Name;

            context.Logger.LogMessage("[DOM] - WATER MODEL VERTEX CHANNEL '" + vertexChannelName + "'");

            // if this vertex channel has an acceptable names, process it as normal.
            if (m_AcceptableVertexChannelNames.Contains(vertexChannelName))
            {
                base.ProcessVertexChannel(geometry, vertexChannelIndex, context);
            }
            // otherwise, remove it from the vertex channels; it's just extra data
            // we don't need.
            else
            {
                geometry.Vertices.Channels.Remove(vertexChannelName);
            }
        }

        #endregion
    }
}
