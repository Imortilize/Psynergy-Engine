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
    public enum ParticleEngineType
    {
        None = 0,
        DPSF = 1,
        Mercury = 2
    };

    public class ParticleEngine : Singleton<ParticleEngine>
    {
        private ContentManager m_ContentManager = null;
        private GraphicsDeviceManager m_Graphics = null;
        private ParticleEngineType m_EngineType = ParticleEngineType.None;
        private ParticleSystem m_ParticleSystem = null;

        public ParticleEngine()
        {
        }

        public ParticleEngine(ContentManager content, GraphicsDeviceManager graphics)
        {
            m_ContentManager = content;
            m_Graphics = graphics;
        }

        public override void Initialise()
        {
            base.Initialise();

            // Create particle system
            CreateParticleSystem();

            if (m_ParticleSystem != null)
                m_ParticleSystem.Initialise();
        }

        private void CreateParticleSystem()
        {
            switch (m_EngineType)
            {
                case ParticleEngineType.None:
                    {
                        // Create particle system
                        m_ParticleSystem = new ParticleSystem(m_ContentManager, m_Graphics);
                    }
                    break;
                case ParticleEngineType.DPSF:
                    {
                        // Create particle system
                        m_ParticleSystem = new ParticleSystemDPSF(m_ContentManager, m_Graphics);
                    }
                    break;
                case ParticleEngineType.Mercury:
                    {
                        // Create particle system
                        m_ParticleSystem = new ParticleSystemMercury(m_ContentManager, m_Graphics);
                    }
                    break;
            }
        }

        public override void Load()
        {
            base.Load();

            if (m_ParticleSystem != null)
                m_ParticleSystem.Load();
        }

        public override void UnLoad()
        {
            base.Load();

            if (m_ParticleSystem != null)
                m_ParticleSystem.UnLoad();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            if (m_ParticleSystem != null)
                m_ParticleSystem.Update(deltaTime);
        }

        public override void Render(GameTime deltaTime)
        {
            base.Render(deltaTime);

            if (m_ParticleSystem != null)
                m_ParticleSystem.Render(deltaTime);
        }

        public ParticleSystemEffect AddParticleSystem(String name)
        {
            return m_ParticleSystem.AddParticleEffect(name);
        }


    #region Setters
        public void SetParticleEngine(ParticleEngineType engineType)
        {
            m_EngineType = engineType;
        }
    #endregion
    }
}
