using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace XnaGame
{
    class Hierarchy : Node
    {
        public Hierarchy(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            for (int i = 0; i < m_ChildrenNodes.Count; i++)
                m_ChildrenNodes.ElementAt(i).Initialise();
        }

        public override void Load()
        {
            for (int i = 0; i < m_ChildrenNodes.Count; i++)
                m_ChildrenNodes.ElementAt(i).Load();
        }

        public override void Reset()
        {
            for (int i = 0; i < m_ChildrenNodes.Count; i++)
                m_ChildrenNodes.ElementAt(i).Reset();
        }

        public override void Update( GameTime deltaTime )
        {
            // Update children
            for (int i = 0; i < m_ChildrenNodes.Count; i++)
                m_ChildrenNodes.ElementAt(i).Update( deltaTime );
        }

        public override void Render(GameTime deltaTime)
        {
            // Render children
            for (int i = 0; i < m_ChildrenNodes.Count; i++)
                m_ChildrenNodes.ElementAt(i).Render(deltaTime);
        }
    }
}
