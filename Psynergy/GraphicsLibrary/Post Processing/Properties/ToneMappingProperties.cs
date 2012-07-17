using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class ToneMappingProperties : AbstractPostProcessorProperties
    {
        public ToneMappingProperties() : base(false)
        {
        }

        public ToneMappingProperties(bool enable) : base(enable)
        {
        }
    }
}
