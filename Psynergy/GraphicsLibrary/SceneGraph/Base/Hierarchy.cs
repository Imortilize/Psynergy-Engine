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
        private GameObject m_RootNode = new GameObject("Root");
        private bool m_Loaded = false;

        public Hierarchy() : base("")
        {
        }

        public Hierarchy(String name) : base(name)
        {
        }

        public override void Initialise()
        {
            // Initialise the hierarchy
            InitialiseHierarchy(m_RootNode);

            base.Initialise();
        }

        private void InitialiseHierarchy(GameObject node)
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
                LoadHierarchy(m_RootNode);

                // Set to loaded
                m_Loaded = true;
            }
        }

        private void LoadHierarchy(GameObject node)
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
            ResetHierarchy(m_RootNode);
        }

        private void ResetHierarchy(GameObject node)
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
                UnLoadHierarchy(m_RootNode);

                // Set to not loaded
                m_Loaded = false;
            }
        }

        private void UnLoadHierarchy(GameObject node)
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
            if ( Active )
            {
                // Update hierarchy
                UpdateHierarchy(m_RootNode, deltaTime);
            }
        }

        private void UpdateHierarchy(GameObject node, GameTime deltaTime)
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
            if (ActiveRender)
            {
                // Update hierarchy
                RenderHierarchy(m_RootNode, deltaTime);
            }
        }

        private void RenderHierarchy(GameObject node, GameTime deltaTime)
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

        #region Find Functions
        public List<T> FindAll<T>() where T : GameObject
        {
            return FindChildren<T>(RootNode);
        }

        private List<T> FindChildren<T>(GameObject node) where T : GameObject
        {
            List<T> toRet = new List<T>();

            foreach (GameObject child in node.Children)
            {
                // If this child has children then process them first
                if (child.Children.Count > 0)
                    toRet.AddRange(FindChildren<T>(child));

                // Check whether the child is of a model or not
                if (child.InheritsFrom<T>())
                    toRet.Add(child as T);
            }

            return toRet;
        }

        public List<GameObject> FindAllByIntreface<T>() where T : IInterface
        {
            return FindChildrenByinterface<T>(RootNode);
        }

        private List<GameObject> FindChildrenByinterface<T>(GameObject node) where T : IInterface
        {
            List<GameObject> toRet = new List<GameObject>();

            foreach (GameObject child in node.Children)
            {
                // If this child has children then process them first
                if (child.Children.Count > 0)
                    toRet.AddRange(FindChildrenByinterface<T>(child));

                // Check whether the child is of a model or not
                if (child.Interfaces<T>())
                    toRet.Add(child);
            }

            return toRet;
        }
        #endregion

        public void AppendChildToTop( GameObject node )
        {
            // Try to append the child to the top
            RootNode.AppendChildToTop( node );
        }

        public GameObject RootNode { get { return m_RootNode; } }
    }
}
