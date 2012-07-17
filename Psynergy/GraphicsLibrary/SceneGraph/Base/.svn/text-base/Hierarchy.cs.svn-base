using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

namespace Psynergy.Graphics
{
    public class Hierarchy : GameObject
    {
        private Node m_BaseNode = new Node("Base");
        private bool m_Loaded = false;

        private bool m_Update = true;
        private bool m_Render = true;

        public Hierarchy() : base("")
        {
        }

        public Hierarchy(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            // Initialise the hierarchy
            InitialiseHierarchy(m_BaseNode);

            base.Initialise();
        }

        private void InitialiseHierarchy(Node node)
        {
            for (int i = 0; i < node.Children.Count; i++)
            {
                node.Children[i].Initialise();

                if (node.Children[i].Children.Count > 0)
                    InitialiseHierarchy(node.Children[i]);
            }
        }

        public override void Load()
        {
            // If not loaded already
            if (!m_Loaded)
            {
                // Load all data for all nodes in this hierarchy
                LoadHierarchy(m_BaseNode);

                // Set to loaded
                m_Loaded = true;
            }
        }

        private void LoadHierarchy(Node node)
        {
            for (int i = 0; i < node.Children.Count; i++)
            {
                node.Children[i].Load();

                if (node.Children[i].Children.Count > 0)
                    LoadHierarchy(node.Children[i]);
            }
        }

        public override void Reset()
        {
            base.Reset();

            // Load all data for all nodes in this hierarchy
            ResetHierarchy(m_BaseNode);
        }

        private void ResetHierarchy(Node node)
        {
            for (int i = 0; i < node.Children.Count; i++)
            {
                node.Children[i].Reset();

                if (node.Children[i].Children.Count > 0)
                    ResetHierarchy(node.Children[i]);
            }
        }

        public override void UnLoad()
        {
            // If loaded already
            if (m_Loaded)
            {
                // UnLoad all data for all nodes in this hierarchy
                UnLoadHierarchy(m_BaseNode);

                // Set to not loaded
                m_Loaded = false;
            }
        }

        private void UnLoadHierarchy(Node node)
        {
            for (int i = 0; i < node.Children.Count; i++)
            {
                node.Children[i].UnLoad();

                if (node.Children[i].Children.Count > 0)
                    UnLoadHierarchy(node.Children[i]);
            }
        }

        public override void Update( GameTime deltaTime )
        {
            if ( m_Update )
            {
                // Update hierarchy
                UpdateHierarchy(m_BaseNode, deltaTime);
            }
        }

        private void UpdateHierarchy(Node node, GameTime deltaTime)
        {
            for (int i = 0; i < node.Children.Count; i++)
            {
                // If this child node has children then render them first
                UpdateHierarchy(node.Children[i], deltaTime);
            }

            // If active and rendering is active for this node then render the node
            if (node.Active)
                node.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            if (m_Render)
            {
                // Update hierarchy
                RenderHierarchy(m_BaseNode, deltaTime);
            }
        }

        private void RenderHierarchy(Node node, GameTime deltaTime)
        {
            for (int i = 0; i < node.Children.Count; i++)
            {
                // If this child node has children then render them first
                RenderHierarchy(node.Children[i], deltaTime);
            }

            // If active and rendering is active for this node then render the node
            if (node.Active && node.ActiveRender)
                node.Render(deltaTime);
        }

        public List<Node> FindAllModels()
        {
            Node startNode = RootNode;

            return NodeChildren(startNode, typeof(ModelNode));
        }

        public List<Node> FindAllRenderNodes()
        {
            Node startNode = RootNode;

            return NodeChildren(startNode, typeof(RenderNode));
        }

        public List<Node> FindAllTerrainNodes()
        {
            Node startNode = RootNode;

            return NodeChildren(startNode, typeof(TerrainNode));
        }

        public List<Node> FindAllLights()
        {
            Node startNode = RootNode;

            return NodeChildren(startNode, typeof(Light));
        }

        private List<Node> NodeChildren(Node node, Type nodeType)
        {
            List<Node> toRet = new List<Node>();

            foreach (Node child in node.Children)
            {
                // If this child has children then process them first
                if (child.Children.Count > 0)
                    toRet.AddRange(NodeChildren(child, nodeType));

                // Check whether the child is of a model or not
                if ((child.GetType() == nodeType) || (child.InheritsFrom(nodeType)))
                    toRet.Add(child);
            }

            return toRet;
        }

        public void AppendChildToTop( Node node )
        {
            // Try to append the child to the top
            RootNode.AppendChildToTop( node );
        }

        public Node RootNode { get { return m_BaseNode; } }
        public bool Active { get { return m_Update; } set { m_Update = value; } }
    }
}
