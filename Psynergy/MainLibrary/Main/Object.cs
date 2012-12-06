using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy
{
    public class GameObject : PropertyBag, IFactoryProduct, IObject, IFocusable
    {
        
        #region Register properties handling (should be fine )
        protected bool m_PropertiesRegistered = false;
        public void RegisterProperties( Factory factory )
        {
            // Check that properties havn't already been registered.
            // Prevents them being registered more then once.
            if ( !m_PropertiesRegistered )
            {
                // Register class properties
                ClassProperties(factory);

                // Handlers
                GetValue += new PropertySpecEventHandler(this.Get_Value);
                SetValue += new PropertySpecEventHandler(this.Set_Value);

                // State the properties have been registered
                m_PropertiesRegistered = true;
            }
        }
        #endregion
        #region Factory Property setting
        protected virtual void ClassProperties(Factory factory)
        {
            factory.RegisterVector3("pos", "TempPosition");
            factory.RegisterVector3("scale", "TempScale");
            factory.RegisterQuat("rot", "TempRotation");
            factory.RegisterQuat("orbitalrot", "OrbitalRotation");
            factory.RegisterVector3("rotdegrees", "TempRotationInDegrees");
            factory.RegisterFloat("opacity", "Opacity");
            factory.RegisterFloat("weight", "Weight");
            factory.RegisterBool("setfocus", "Focused");
            factory.RegisterString("parent", "ParentName");
            factory.RegisterClass(typeof(Controller), "controller", "Controller");  
        }
        #endregion

        #region Basic Attributes
        protected String  m_Name = "";
        protected Type    m_Type;
        protected float   m_Weight  = 0.0f;
        protected float   m_Opacity = 1.0f;
        #endregion

        #region Activity Flags
        private bool m_Active = true;       // Flags whether to update and render an object
        private bool m_Render = true;       // Flags whether to render an object
        private bool m_Refresh = false;
        #endregion

        #region Forces (Temp)
        protected const float GRAVITY  = 9.8f;         // Constant gravity
        protected const float FRICTION = 2.0f;         // Constant friction ( for now )
        protected const float AIRRESISTANCE = 3.0f;    // Constant air resistance ( for now )
        #endregion

        #region Transforms
        // Transforms
        private Transform m_Transform = null;
        private Transform m_StartTransform = null;
        #endregion

        #region Focus
        // Focus on object
        private bool m_Focused = false;
        #endregion

        #region Parent and Child containers
        private String m_ParentName = null;
        private GameObject m_ParentNode = null;
        private List<GameObject> m_ChildrenNodes = new List<GameObject>();
        private GameObject m_NextSibling = null;
        private GameObject m_PreviousSibling = null;
        #endregion

        #region Controller
        // This is what is used to control each object ( a behaviour as such )
        private Controller m_Controller = null;
        #endregion

        #region Constructors
        public GameObject()
        {
            m_Name = "";
            m_Type = this.GetType();

            // Create transforms
            m_Transform = new Transform(this);
            m_StartTransform = new Transform(this);
        }

        public GameObject(String name)
        {
            m_Name = name;
            m_Type = this.GetType();

            // Create transforms
            m_Transform = new Transform(this);
            m_StartTransform = new Transform(this);
        }

        public GameObject(GameObject copyFrom)
		{
			PropertySpec[] copyArray = new PropertySpec[copyFrom.Properties.Count];
			copyFrom.Properties.CopyTo(copyArray);

			Properties.Clear();
			Properties.AddRange(copyArray);

			for(int p = 0; p < Properties.Count; ++p)
			{
				PropertySpecEventArgs args1 = new PropertySpecEventArgs(copyFrom.Properties[p], null);
				copyFrom.Get_Value(this, args1); 

				PropertySpecEventArgs args2 = new PropertySpecEventArgs(Properties[p], args1.Value);
				Set_Value(this, args2);
			}

            // Copy transforms
            m_Transform = copyFrom.transform;
            m_StartTransform = copyFrom.startTransform;
		}
        #endregion

        public virtual void Initialise()
        {
            // If a controller doesn't exist then create a basic controller and intialise it
            // Otherwise just initialise it
            if (m_Controller != null)
                m_Controller.Initialise();

            // Set the start transform
            startTransform = transform;

            // Reset object accordingly
            Reset();
        }

        /* Called after properties have been set */
        public virtual void DefaultProperties()
        {
        }

        public virtual void Load()
        {
        }

        public virtual void UnLoad()
        {
        }

        public virtual void Reset()
        {
            // Reset the controller
            if (m_Controller != null)
                m_Controller.Reset();

            // Reset transform to start transform
            transform = startTransform;

            // Refresh object
            m_Refresh = true;
        }

        #region Update
        public virtual void Update(GameTime deltaTime)
        {
            // If this object is to update
            if (m_Refresh)
                UpdateSiblings();

            // Update the controller
            if (m_Controller != null)
                m_Controller.Update(deltaTime);

            // Update transform
            m_Transform.Rebuild();
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
        #endregion

        #region Render
        public virtual void Render(GameTime deltaTime)
        {
        }
        #endregion

        #region String Setter Functions
        protected String[] SplitString(String vector)
        {
            return vector.Split(new Char[] { ' ' });
        }

        /* Used to set a nodes position from the xml file right now */
        public void SetPos(String pos)
        {
            String[] splitPos = SplitString(pos);

            // Assert if formatted wrong
            Debug.Assert(splitPos.Length == 3, "Positions should be 3 numbers long!");

            // Save the position accordingly
            transform.Position = new Vector3(Convert.ToSingle(splitPos[0]), Convert.ToSingle(splitPos[1]), Convert.ToSingle(splitPos[2]));
        }
        #endregion

        #region Parent and Child container Functions
        #region Add
        public void AddChild(GameObject child)
        {
            // First check to make sure this isn't a child of this object already
            if (!m_ChildrenNodes.Contains(child))
            {
                // Now remove this child from any existing parents as it isn't this object
                if (child.Parent != null)
                    child.Parent.RemoveChild(child);

                // Set the childs parent to be this object
                child.Parent = this;

                // Add as a child to thie object
                m_ChildrenNodes.Add(child);
            }
        }

        public void AppendChildToTop(GameObject node)
        {
            if (m_ChildrenNodes.Contains(node))
            {
                m_ChildrenNodes.Remove(node);
                m_ChildrenNodes.Insert(0, node);
            }
        }
        #endregion

        #region Remove
        public bool RemoveChild(GameObject child)
        {
            bool result = false;

            if (m_ChildrenNodes.Contains(child))
            {
                result = m_ChildrenNodes.Remove(child);

                // Destory all links from this child
                child.Parent = null;
                child.NextSibling = null;
                child.PreviousSibling = null;

                // Child was removed
                OnChildRemoved(child, result);

                // Set to refresh hierarchy
                m_Refresh = true;
            }

            return result;
        }

        public void RemoveChild(int index)
        {
            if (index < m_ChildrenNodes.Count)
            {
                GameObject child = m_ChildrenNodes[index];

                // Remove the child
                RemoveChild(child);
            }
        }

        protected virtual void OnChildRemoved(GameObject child, bool result)
        {
        }
        #endregion

        #region Find
        public GameObject FindChild(GameObject child)
        {
            GameObject result = null; 

            // Try and find the parent
            foreach (GameObject childObject in child.Children)
            {
                // Search through all children to look for this child
                result = FindChild(childObject);

                if (result != null)
                    break;
            }

            if (child.Children.Count > 0)
                result = child.Children.Find(delegate(GameObject childObject) { return childObject == child; });

            return result;
        }

        public GameObject FindChild(GameObject child, String name)
        {
            GameObject result = null;

            // Try and find the parent
            foreach (GameObject childObject in child.Children)
            {
                // Search through all children to look for this child
                result = FindChild(childObject, name);

                if (result != null)
                    return result;
            }

            if (child.Children.Count > 0)
                result = child.Children.Find(delegate(GameObject childObject) { return childObject.Name == name; });

            return result;
        }

        public GameObject FindChild(String name)
        {
            return FindChild(this, name);
        }
        #endregion
        #endregion

        #region Type comparers
        public bool IsTypeOf<T>() where T : GameObject
        {
            return (m_Type == typeof(T));
        }
        
        public bool InheritsFrom<T>() where T : GameObject
        {
            return (m_Type.IsSubclassOf(typeof(T)) || m_Type == typeof(T));
        }
        #endregion

        #region IFactoryProduct Implementation
        public object GetFactoryKey()
        {
            return Factory.Instance.GetKey(m_Type);
        }
        #endregion

        #region Get/ Set functions

        private void Get_Value(object sender, PropertySpecEventArgs e)
        {
            Type t = GetType();

            PropertyInfo[] PropInfo = t.GetProperties();

            foreach (PropertyInfo p in PropInfo)
            {
                if (p.Name.Equals(e.Property.Name))
                {
                    e.Value = p.GetValue(this, null);
                    return;
                }
            }
        }

        public void SetProperty(object sender, PropertySpecEventArgs e)
        {
            if (!m_PropertiesRegistered)
                Set_Value(sender, e);
        }
        private void Set_Value(object sender, PropertySpecEventArgs e)
        {
            Type t = GetType();

            PropertyInfo[] PropInfo = t.GetProperties();

            foreach (PropertyInfo p in PropInfo)
            {
                if (p.Name.Equals(e.Property.Name))
                {
                    p.SetValue(this, e.Value, null);
                    return;
                }
            }
        }

        public void SetScale(String scale)
        {
            String[] splitScale = SplitString(scale);

            // Assert if formatted wrong
            Debug.Assert(splitScale.Length == 3, "Positions should be 3 numbers long!");

            // Set scale
            transform.Scale = new Vector3(Convert.ToSingle(splitScale[0]), Convert.ToSingle(splitScale[1]), Convert.ToSingle(splitScale[2]));
        }

        public virtual void SetOpacity(float opacity)
        {
            m_Opacity = opacity;
        }
        #endregion

        #region Transform Update
        public virtual void OnTransformUpdated()
        {
        }
        #endregion

        #region Factory class registers
        public virtual void OnClassSet(GameObject invokeNode)
        {
        }
        #endregion

        #region Properties
        // Basic Properties
        public String Name { get { return m_Name; } set { m_Name = value; } }
        public Type Type { get { return m_Type; } set { m_Type = value; } }
        public float Weight
        {
            get { return m_Weight; }
            set
            {
                // Weights are clamped between 0 and 100
                value = MathHelper.Clamp(value, 0, 100);

                // Set the object weight
                m_Weight = value;
            }
        }


        public float Opacity { get { return m_Opacity; } set { m_Opacity = value; SetOpacity(value); } }

        // Activity Flags
        public bool Active { get { return m_Active; } set { m_Active = value; } }
        public bool ActiveRender { get { return m_Render; } set { m_Render = value; } }

        // Transforms
        public Transform transform { get { return m_Transform; } set { m_Transform = value; } }
        public Transform startTransform { get { return m_StartTransform; } set { m_StartTransform = value; } }

        // Parent and Child container properties
        public String ParentName { get { return m_ParentName; } set { m_ParentName = value; } }

        [CategoryAttribute("Node:Properties"), DescriptionAttribute("parent")]
        public GameObject Parent { get { return m_ParentNode; } set { m_ParentNode = value; } }
        public GameObject NextSibling { get { return m_NextSibling; } set { m_NextSibling = value; } }
        public GameObject PreviousSibling { get { return m_PreviousSibling; } set { m_PreviousSibling = value; } }
        public List<GameObject> Children { get { return m_ChildrenNodes; } }

        // Whether this object is focused or not
        public bool Focused { get { return m_Focused; } set { m_Focused = value; } }

        // Controller
        public Controller Controller { get { return m_Controller; } set { m_Controller = value; } }
        public bool HasController { get { return (m_Controller != null); } }

        // TEMPORARY
        public Vector3 TempPosition { get { return transform.Position; } set { transform.Position = value; } }
        public Vector3 TempScale { get { return transform.Scale; } set { transform.Scale = value; } }
        public Quaternion TempRotation { get { return transform.Rotation; } set { transform.Rotation = value; } }
        public Quaternion TempOrbitalRotation { get { return transform.OrbitalRotation; } set { transform.OrbitalRotation = value; } }
        public Vector3 TempRotationInDegrees { get { return Vector3.Zero; } set { transform.RotationInDegrees = value; } }
        #endregion
    }
}
