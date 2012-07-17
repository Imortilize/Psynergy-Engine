using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy.Graphics
{
    public class PivotNode : Node3D
    {
        protected Quaternion m_CurrentRotation = Quaternion.Identity;

        public PivotNode() : base("")
        {
        }

        public PivotNode(String name)
            : base(name)
        {
        }

        public Quaternion CurrentRotation { get { return m_CurrentRotation; } set { m_CurrentRotation = value; } }
    }
}
