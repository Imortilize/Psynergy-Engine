using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;

namespace Psynergy.WaterPipeline
{
    public class WaterContent
    {
        private float m_WaterScale;

        public WaterContent(float waterScale)
        {
            m_WaterScale = waterScale;
        }

        #region Property Set / Gets
        public float WaterScale { get { return m_WaterScale; } }
        #endregion
    }

    [ContentTypeWriter]
    public class WaterInfoWriter : ContentTypeWriter<WaterContent>
    {
        protected override void Write(ContentWriter output, WaterContent value)
        {
            output.Write(value.WaterScale);
        }

        /// <summary>
        /// Tells the content pipeline what CLR type the
        /// data will be loaded into at runtime.
        /// </summary>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "Psynergy.WaterPipeline.WaterInfo, " +
                "Psynergy.WaterPipeline, Version=1.0.0.0, Culture=neutral";
        }

        /// <summary>
        /// Tells the content pipeline what worker type
        /// will be used to load the data.
        /// </summary>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Psynergy.WaterPipeline.WaterInfoReader, " +
                "Psynergy.WaterPipeline, Version=1.0.0.0, Culture=neutral";
        }
    }
}
