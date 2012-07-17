using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy
{
    public class GameObject : PropertyBag, IFactoryProduct, IObject
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
            factory.RegisterVector3("pos", "Position");
            factory.RegisterVector3("scale", "Scale");
            factory.RegisterFloat("opacity", "Opacity");
        }
        #endregion

        protected String  m_Name = "";
        protected Type    m_Type;
        protected float   m_Weight  = 0.0f;
        protected float   m_Opacity = 1.0f;
        protected Vector3 m_Scale         = new Vector3(1.0f, 1.0f, 1.0f);
        private   Vector3 m_ActualScale   = new Vector3(1.0f, 1.0f, 1.0f);   // Actual scale used by this object after heirarchy transgression

        protected const float GRAVITY  = 9.8f;         // Constant gravity
        protected const float FRICTION = 2.0f;         // Constant friction ( for now )
        protected const float AIRRESISTANCE = 3.0f;    // Constant air resistance ( for now )

        #region Constructors
        public GameObject()
        {
            m_Name = "";
            m_Type = this.GetType();
        }

        public GameObject(String name)
        {
            m_Name = name;
            m_Type = this.GetType();
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
		}
        #endregion

        public virtual void Initialise()
        {
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
        }

        public virtual void Update(GameTime deltaTime)
        {
        }

        public virtual void Render(GameTime deltaTime)
        {
        }

        protected String[] SplitString(String vector)
        {
            return vector.Split(new Char[] { ' ' });
        }

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

        public void SetScale(Vector3 scale)
        {
            m_Scale = scale;
        }

        public void SetScale(String scale)
        {
            String[] splitScale = SplitString(scale);

            // Assert if formatted wrong
            Debug.Assert(splitScale.Length == 3, "Positions should be 3 numbers long!");

            // Save the position accordingly
            m_Scale.X = System.Convert.ToSingle(splitScale[0]);
            m_Scale.Y = System.Convert.ToSingle(splitScale[1]);
            m_Scale.Z = System.Convert.ToSingle(splitScale[2]);
        }

        public Vector2 GetScale2D()
        {
            return new Vector2(m_Scale.X, m_Scale.Y);
        }

        public Vector3 GetScale()
        {
            return m_Scale;
        }

        public virtual void SetOpacity(float opacity)
        {
            m_Opacity = opacity;
        }
        #endregion

        #region Factory class registers
        public virtual void OnClassSet(GameObject invokeNode)
        {
        }
        #endregion

        #region Properties
        public String Name { get { return m_Name; } set { m_Name = value; } }
        public Type Type { get { return m_Type; } set { m_Type = value; } }

        public float Opacity { get { return m_Opacity; } set { m_Opacity = value; SetOpacity(value); } }
        public Vector3 Scale { get { return m_Scale; } set { m_Scale = value; } }
        public float ScaleX { get { return m_Scale.X; } set { m_Scale.X = value; } }
        public float ScaleY { get { return m_Scale.Y; } set { m_Scale.Y = value; } }
        public float ScaleZ { get { return m_Scale.Z; } set { m_Scale.Z = value; } }
        public Vector3 ActualScale { get { return m_ActualScale; } set { m_ActualScale = value; } }
        #endregion
    }
}
