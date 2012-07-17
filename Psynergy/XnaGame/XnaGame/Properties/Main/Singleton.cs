using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace XnaGame
{
    public class Singleton<T> where T : class, new()
    {
        #region Member Variables
        private static T m_Instance = null;
        #endregion

        public Singleton()
        {
            Debug.Assert(m_Instance == null, "Singleton means 1, and 1 only");
            m_Instance = this as T;

            // Call create function
            Setup();
        }

        public static T Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public virtual void Initialise()
        {
        }

        protected virtual void Setup()
        {
        }

        public virtual void Reset()
        {
        }

        public virtual void Load()
        {
        }

        public virtual void UnLoad()
        {
        }

        public virtual void Update( GameTime deltaTime )
        {
        }
    }
}
