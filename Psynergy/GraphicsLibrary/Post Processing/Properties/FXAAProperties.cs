using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class FXAAProperties : AbstractPostProcessorProperties
    {
        public FXAAProperties()
            : base(false)
        {
        }

        public FXAAProperties(bool enable)
            : base(enable)
        {
        }
    }
}
