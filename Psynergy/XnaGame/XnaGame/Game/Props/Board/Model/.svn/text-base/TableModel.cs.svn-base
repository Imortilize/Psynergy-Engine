using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

/* Graphics Library */
using Psynergy.Graphics;

namespace XnaGame
{
    class TableModel : ModelNode
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            base.ClassProperties(factory);
        }
        #endregion

        public TableModel() : base("")
        {
        }

        public TableModel(String name) : base(name)
        {
        }

        public virtual void Enable()
        {
            ActiveRender = true;
        }

        public virtual void Disable()
        {
            ActiveRender = false;
        }
    }
}
