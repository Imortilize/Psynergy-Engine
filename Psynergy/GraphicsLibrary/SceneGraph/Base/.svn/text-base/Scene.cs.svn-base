using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/* Main Library */
using Psynergy;

namespace Psynergy.Graphics
{
    public class Scene : GameObject
    {
        private SceneFragment m_Resource = null;
        private Hierarchy m_Hierarchy = null;

        public Scene() : base( "" )
        {
            m_Hierarchy = new Hierarchy();
        }

        public Scene( String name ): base(name)
        {
            m_Hierarchy = new Hierarchy(name + ":Hierarchy");
        }

        public Scene(String name, String resource) : base(name)
        {
            m_Hierarchy = new Hierarchy(name + ":Hierarchy");

            if ( resource != null )
                m_Resource = new SceneFragment(resource, m_Hierarchy);
        }

        public override void Initialise()
        {
            // Load resources
            if (m_Resource != null)
                m_Resource.Load();

            // Initialise the hierarchy
            if ( m_Hierarchy != null )
                m_Hierarchy.Initialise();

            // Initialise children
            base.Initialise();
        }

        public override void Load()
        {
            // Load resources
            if (m_Hierarchy != null)
                m_Hierarchy.Load();
        }

        public override void Reset()
        {
            base.Reset();

            // reset the hierarchy
            if (m_Hierarchy != null)
                m_Hierarchy.Reset();
        }

        public override void UnLoad()
        {
            // Load resources
            if (m_Hierarchy != null)
                m_Hierarchy.UnLoad();
        }

        public override void Update(GameTime deltaTime)
        {
            // Load resources
            if (m_Hierarchy != null)
                m_Hierarchy.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            // Load resources
            if (m_Hierarchy != null)
                m_Hierarchy.Render(deltaTime);
        }

        public Hierarchy Hierarchy { get { return m_Hierarchy; } }
    }
}
