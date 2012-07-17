using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaGame
{
    class Node : Object
    {
        protected Node m_ParentNode = null;
        protected List<Node> m_ChildrenNodes = null;
        protected Node m_NextSibling = null;
        protected Node m_PreviousSibling = null;

        bool m_Refresh = false;

        public Node( String name ) : base( name )
        {
            m_ChildrenNodes = new List<Node>();
        }

        public override void Initialise()
        {
            for (int i = 0; i < m_ChildrenNodes.Count; i++)
                m_ChildrenNodes[i].Initialise();

            base.Initialise();
        }

        public override void Reset()
        {
            // Reset all children
            for (int i = 0; i < m_ChildrenNodes.Count; i++)
                m_ChildrenNodes[i].Reset();
        }

        public override void Load()
        {
        }

        public void AddChild(Node child)
        {
            m_ChildrenNodes.Add(child);

            // Set the childs parent
            child.Parent = this;

            // Set to refresh the hierarchy
            m_Refresh = true;
        }

        public void RemoveChild(Node child)
        {
            bool result = m_ChildrenNodes.Remove(child);

            // No longer has a parent or siblings
            if (result)
            {
                child.Parent = null;
                child.NextSibling = null;
                child.PreviousSibling = null;

                // Set to refresh the hierarchy
                m_Refresh = true;
            }
        }

        public void RemoveChild(int index)
        {
            if (index < m_ChildrenNodes.Count)
            {
                // Remove parent
                m_ChildrenNodes[index].Parent = null;

                m_ChildrenNodes.RemoveAt(index);
            }
        }

        public Node FindChild(Node child)
        {
            Node result = m_ChildrenNodes.Find( delegate(Node childNode) { return childNode == child; } );

            return result;
        }

        public Node FindChild(String name)
        {
            Node result = m_ChildrenNodes.Find(delegate(Node childNode) { return childNode.Name == name; });

            return result;
        }

        public override void Update( GameTime deltaTime )
        {
            if (m_Refresh)
                UpdateHierarchy();

            // Update all children
            for (int i = 0; i < m_ChildrenNodes.Count; i++)
                m_ChildrenNodes[i].Update( deltaTime );
        }

        protected void UpdateHierarchy()
        {
            // Update siblings accordingly
            for (int i = 0; i < m_ChildrenNodes.Count; i++)
            {
                m_ChildrenNodes[i].NextSibling = null;

                if (i > 0)
                {
                    m_ChildrenNodes[i].PreviousSibling = m_ChildrenNodes[i - 1];
                    m_ChildrenNodes[i - 1].NextSibling = m_ChildrenNodes[i];
                }
                
            }
        }

        public override void Render(GameTime deltaTime)
        {
            // Draw all children
            for (int i = 0; i < m_ChildrenNodes.Count; i++)
                m_ChildrenNodes[i].Render(deltaTime);
        }

        public Node Parent { get { return m_ParentNode; } set { m_ParentNode = value; } }
        public Node NextSibling { get { return m_NextSibling; } set { m_NextSibling = value; } }
        public Node PreviousSibling { get { return m_PreviousSibling; } set { m_PreviousSibling = value; } }
    }
}
