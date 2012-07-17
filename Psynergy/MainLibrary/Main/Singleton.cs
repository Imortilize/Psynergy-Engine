using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Psynergy
{
    public class Singleton<T> where T : class, new()
    {
        #region Member Variables
        private static volatile T m_Instance = null;

        // Static synchronization root object, for locking
        private static object m_SyncRoot = new object();
        #endregion

        public Singleton()
        {
            // Lock the object
            lock (m_SyncRoot)
            {
                Debug.Assert(m_Instance == null, "Singleton means 1, and 1 only");
                m_Instance = this as T;
            }

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
            // Register node events
            RegisterEvents();
        }

        protected virtual void RegisterEvents()
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

        public virtual void Render(GameTime deltaTime)
        {
        }
    }
}
