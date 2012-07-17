using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Psynergy;

namespace Psynergy.Graphics
{
    public class ParticleSystem
    {
        protected ContentManager m_ContentManager = null;
        protected GraphicsDeviceManager m_Graphics = null;
        protected String m_BaseDirectory = "ParticleEffects";
        protected List<ParticleSystemEffect> m_Particles = new List<ParticleSystemEffect>();
        protected List<ParticleSystemEffect> m_ActiveParticles = new List<ParticleSystemEffect>();

        public ParticleSystem(ContentManager content, GraphicsDeviceManager graphics)
        {
            m_ContentManager = content;
            m_Graphics = graphics;
        }

        public virtual void Initialise()
        {
        }

        public virtual void Reset()
        {
            m_Particles.Clear();
            m_ActiveParticles.Clear();
        }

        public virtual void Load()
        {
        }

        public virtual void UnLoad()
        {
        }

        public virtual void Update(GameTime deltaTime)
        {
            foreach (ParticleSystemEffect particleEffect in m_ActiveParticles)
                particleEffect.Update(deltaTime);
        }

        public virtual void Render(GameTime deltaTime)
        {
            foreach (ParticleSystemEffect particleEffect in m_ActiveParticles)
                particleEffect.Render(deltaTime);
        }

        public virtual ParticleSystemEffect AddParticleEffect(String particleEffectName)
        {
            return AddParticleEffect(new ParticleSystemEffect(m_ContentManager, particleEffectName));
        }

        public ParticleSystemEffect AddParticleEffect(ParticleSystemEffect particleEffect)
        {
            if (particleEffect != null)
            {
                // Set the particle system that this particle system effect is part of 
                particleEffect.ParticleSystem = this;

                // Initialise
                particleEffect.Initialise();

                // Load
                particleEffect.Load();

                // Add
                m_Particles.Add(particleEffect);
            }

            return particleEffect;
        }

        public void AddActiveParticle(ParticleSystemEffect particleEffect)
        {
            Debug.Assert(particleEffect != null, "[WARNING] - PARTICLE EFFECT MUST NOT BE NULL WHEN BEING ADDED AS AN ACTIVE PARTICLE!");

            if (particleEffect != null)
            {
                if ( !m_ActiveParticles.Contains(particleEffect) )
                    m_ActiveParticles.Add(particleEffect);
            }
        }

        public void RemoveActiveParticle(ParticleSystemEffect particleEffect)
        {
            Debug.Assert(particleEffect != null, "[WARNING] - PARTICLE EFFECT MUST NOT BE NULL WHEN BEING REMOVING AS AN ACTIVE PARTICLE!");

            if (particleEffect != null)
            {
                if (m_ActiveParticles.Contains(particleEffect))
                    m_ActiveParticles.Remove(particleEffect);
            }
        }

        public bool ContainsActiveParticle(ParticleSystemEffect particleEffect)
        {
            Debug.Assert(particleEffect != null, "[WARNING] - PARTICLE EFFECT MUST NOT BE NULL WHEN CHECKING FOR AN ACTIVE PARTICLE!");

            bool toRet = false;

            if (particleEffect != null)
                toRet = m_ActiveParticles.Contains(particleEffect);

            return toRet;
        }
    }
}
