using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Psynergy.Graphics
{
    public class ParticleSystemEffect
    {
        protected ContentManager m_Content = null;
        protected String m_Name = "";
        protected String m_BaseDirectory = "ParticleEffects/";

        // A reference to the particle system that this particle is apart of 
        private ParticleSystem m_ParticleSystem = null;

        public ParticleSystemEffect(ContentManager content, String name)
        {
            m_Content = content;
            m_Name = name;
        }

        public virtual void Initialise()
        {
        }

        public virtual void Load()
        {
        }

        public virtual void UnLoad()
        {
        }

        public virtual bool Trigger(Vector3 pos)
        {
            bool toRet = (m_ParticleSystem != null);

            if (toRet)
            {
                // Add the particle to the active particle list
                m_ParticleSystem.AddActiveParticle(this);
            }

            return (m_ParticleSystem != null);
        }

        public virtual void Update(GameTime deltaTime)
        {
        }

        public virtual void Render(GameTime deltaTime)
        {
        }

        #region Property Set / Gets
        public ParticleSystem ParticleSystem { get { return m_ParticleSystem; } set { m_ParticleSystem = value; } }
        #endregion
    }
}
