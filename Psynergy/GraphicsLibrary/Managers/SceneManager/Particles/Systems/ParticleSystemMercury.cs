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
using Psynergy.Input;
using Psynergy.Camera;

/* Mercury */
using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;

namespace Psynergy.Graphics
{
    public class ParticleSystemMercury : ParticleSystem
    {
        private AbstractRenderer m_Renderer = null;

        public ParticleSystemMercury(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
        {
            m_BaseDirectory += "/Mercury";
        }

        public override void Initialise()
        {
            if (m_Graphics != null)
            {
                m_Renderer = new SpriteBatchRenderer
                {
                    GraphicsDeviceService = m_Graphics,
                    Transformation = Matrix.CreateTranslation((m_Graphics.GraphicsDevice.Viewport.Width * 0.5f), (m_Graphics.GraphicsDevice.Viewport.Height * 0.5f), 0f)
                };

                m_Renderer = new ProjectMercury.Renderers.QuadRenderer(10000)
                {
                    GraphicsDeviceService = m_Graphics,
                };
            }

            // Test ( add particle )
            //AddParticleEffect("magictrail");
        }

        public override void Load()
        {
            if (m_ContentManager != null)
            {
                // Load renderer content
                if ( m_Renderer != null )
                    m_Renderer.LoadContent(m_ContentManager);
            }
        }

        public override ParticleSystemEffect AddParticleEffect(String particleEffectName)
        {
            MercuryParticleEffect particleEffect = new MercuryParticleEffect(m_ContentManager, particleEffectName, m_Renderer);

            // Run through initialising process
            return base.AddParticleEffect(particleEffect);
        }
    }
}
