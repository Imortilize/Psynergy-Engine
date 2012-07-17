using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

using Microsoft.Xna.Framework;

namespace Psynergy
{
    public class Node : GameObject, IFocusable
    {
        #region Factory Property setting
        protected override void ClassProperties(Factory factory)
        {
            factory.RegisterString("parent", "ParentName");
            factory.RegisterClass(typeof(Controller), "controller", "Controller");  

            base.ClassProperties(factory);
        }
        #endregion
        
        private bool m_Active = true;       // Flags whether to update and render an object
        private bool m_Render = true;       // Flags whether to render an object
        private bool m_Refresh = false;

        protected String m_ParentName = null;
        protected Node m_ParentNode = null;
        protected List<Node> m_ChildrenNodes = null;
        protected Node m_NextSibling = null;
        protected Node m_PreviousSibling = null;

        protected Vector3 m_Position = new Vector3(0.0f, 0.0f, 0.0f);
        protected Vector3 m_StartPosition = new Vector3(0.0f, 0.0f, 0.0f);

        // Whether to be focused by the camera or not
        protected bool m_CameraFocus = false;

        #region Controller
        protected Controller m_Controller = null;
        #endregion

        public Node() : base( "" )
        {
            m_ChildrenNodes = new List<Node>();
        }

        public Node( String name ) : base( name )
        {
            m_ChildrenNodes = new List<Node>();
        }

        /* Called after properties have been set */
        public override void DefaultProperties()
        {
            if (m_Controller == null)
                m_Controller = DefaultController();
        }

        protected virtual Controller DefaultController()
        {
            return new Controller();
        }

        public override void Initialise()
        {
            // Set the start position
            m_StartPosition = m_Position;

            // Intialise the controller
            InitialiseController();

            base.Initialise();
        }

        protected void InitialiseController()
        {
            // If a controller doesn't exist then create a basic controller and intialise it
            // Otherwise just initialise it
            if (m_Controller != null)
                m_Controller.Initialise();
        }

        public override void Reset()
        {
            Position = m_StartPosition;

            // Reset the controller
            if ( m_Controller != null )
                m_Controller.Reset();

            base.Reset();

            m_Refresh = true;
        }

        public override void Load()
        {
            base.Load();
        }

        public void AddChild(Node child)
        {
            if (!m_ChildrenNodes.Contains(child))
            {
                m_ChildrenNodes.Add(child);

                // Set the childs parent
                child.Parent = this;
            }
        }

        public virtual bool RemoveChild(Node child)
        {
            bool result = false;

            if (m_ChildrenNodes.Contains(child))
            {
                result = m_ChildrenNodes.Remove(child);

                // Destory all links from this child
                child.Parent = null;
                child.NextSibling = null;
                child.PreviousSibling = null;

                // Set to refresh hierarchy
                m_Refresh = true;
            }

            return result;
        }

        public void RemoveChild(int index)
        {
            if (index < m_ChildrenNodes.Count)
            {
                // Remove parent
                m_ChildrenNodes[index].Parent = null;

                m_ChildrenNodes.RemoveAt(index);

                m_Refresh = true;
            }
        }

        public Node FindChild(Node child)
        {
            Node result = null; // m_ChildrenNodes.Find(delegate(Node childNode) { return childNode == child; });

            // Try and find the parent
            foreach (Node childNode in child.Children)
            {
                // Search through all children to look for this child
                result = FindChild(childNode);

                if (result != null)
                    break;
            }

            if (child.Children.Count > 0)
                result = child.Children.Find(delegate(Node childNode) { return childNode == child; });

            return result;
        }

        public Node FindChild(Node child, String name)
        {
            Node result = null; // m_ChildrenNodes.Find(delegate(Node childNode) { return childNode == child; });

            // Try and find the parent
            foreach (Node childNode in child.Children)
            {
                // Search through all children to look for this child
                result = FindChild(childNode, name);

                if (result != null)
                    return result;
            }

            if (child.Children.Count > 0)
                result = child.Children.Find(delegate(Node childNode) { return childNode.Name == name; });

            return result;
        }

        public Node FindChild(String name)
        {
            return FindChild(this, name);
        }

        public void AppendChildToTop(Node node)
        {
            if (m_ChildrenNodes.Contains(node))
            {
                m_ChildrenNodes.Remove(node);
                m_ChildrenNodes.Insert(0, node);
            }
        }

        public override void Update( GameTime deltaTime )
        {
            if (m_Refresh)
                UpdateSiblings();

            // Update the controller
            if ( m_Controller != null )
                m_Controller.Update(deltaTime);
        }

        private void UpdateSiblings()
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
            // Update controller rendering ( mainly debug information )
            //RenderController(deltaTime);
        }

        private void RenderController(GameTime deltaTime)
        {
            if ( m_Controller != null )
                m_Controller.Render(deltaTime);
        }

        public bool InheritsFrom(Type type)
        {
            return (m_Type.IsSubclassOf(type) || m_Type == typeof(Type));
        }

        public virtual void SetPos(Vector3 pos)
        {
            m_Position = pos;
        }

        public void SetPos(String pos)
        {
            String[] splitPos = SplitString(pos);

            // Assert if formatted wrong
            Debug.Assert(splitPos.Length == 3, "Positions should be 3 numbers long!");

            // Save the position accordingly
            m_Position.X = System.Convert.ToSingle(splitPos[0]);
            m_Position.Y = System.Convert.ToSingle(splitPos[1]);
            m_Position.Z = System.Convert.ToSingle(splitPos[2]);
        }

        public Vector2 GetPos2D()
        {
            return new Vector2(m_Position.X, m_Position.Y);
        }

        public virtual Vector3 GetPos()
        {
            return m_Position;
        }

        public virtual float GetMovementSpeed()
        {
            return m_Controller.GetMovementSpeed();
        }

        #region Set/ Gets
        public bool Active { get { return m_Active; } set { m_Active = value; } }
        public bool ActiveRender { get { return m_Render; } set { m_Render = value; } }
        public String ParentName { get { return m_ParentName; } set { m_ParentName = value; } }

        [CategoryAttribute("Node:Properties"), DescriptionAttribute("parent")]
        public Node Parent { get { return m_ParentNode; } set { m_ParentNode = value; } }
        
        public Node NextSibling { get { return m_NextSibling; } set { m_NextSibling = value; } }
        public Node PreviousSibling { get { return m_PreviousSibling; } set { m_PreviousSibling = value; } }
        public List<Node> Children { get { return m_ChildrenNodes; } }

        public virtual Vector3 Position { get { return m_Position; } set { m_Position = value; } }
        public virtual Vector3 StartPosition { get { return m_StartPosition; } set { m_StartPosition = value; } }
        public float PosX { get { return m_Position.X; } set { m_Position.X = value; } }
        public float PosY { get { return m_Position.Y; } set { m_Position.Y = value; } }
        public float PosZ { get { return m_Position.Z; } set { m_Position.Z = value; } }

        public bool Focused { get { return m_CameraFocus; } set { m_CameraFocus = value; } }
        public Controller Controller { get { return m_Controller; } set { m_Controller = value; } }
        #endregion
    }
}
