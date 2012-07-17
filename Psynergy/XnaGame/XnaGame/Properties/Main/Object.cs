using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaGame
{
    class Object
    {
        protected String m_Name = "";
        protected Type m_Type;

        public Object( String name )
        {
            m_Name = name;
            m_Type = this.GetType();
        }

        public virtual void Initialise()
        {
            // Reset object accordingly
            Reset();
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

        public String Name { get { return m_Name; } set { m_Name = value; } }
        public Type Type { get { return m_Type; } set { m_Type = value; } }
    }
}
