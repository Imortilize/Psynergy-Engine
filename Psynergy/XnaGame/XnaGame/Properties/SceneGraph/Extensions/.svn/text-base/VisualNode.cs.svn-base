using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace XnaGame
{
    class VisualNode : Node
    {
        Vector3 m_Scale;

        public VisualNode( String name ) : base( name )
        {
        }

        public override void Initialise()
        {
            m_Scale = new Vector3(1.0f, 1.0f, 1.0f);

            base.Initialise();
        }

        public Vector3 Scale { get { return m_Scale; } set { m_Scale = value; } }
        public float ScaleX { get { return m_Scale.X; } set { m_Scale.X = value; } }
        public float ScaleY { get { return m_Scale.Y; } set { m_Scale.Y = value; } }
        public float ScaleZ { get { return m_Scale.Z; } set { m_Scale.Z = value; } }
    }
}
