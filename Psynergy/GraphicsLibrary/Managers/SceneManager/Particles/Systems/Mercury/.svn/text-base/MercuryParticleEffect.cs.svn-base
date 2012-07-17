using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public class MercuryParticleEffect : ParticleSystemEffect
    {
        private AbstractRenderer m_Renderer = null;
        private ParticleEffect m_ParticleEffect = null;

        public MercuryParticleEffect(ContentManager content, String particleName, AbstractRenderer renderer) : base(content, particleName)
        {
            m_Renderer = renderer;
            m_BaseDirectory += "Mercury/";
        }

        public override void Initialise()
        {
            base.Initialise();

            m_ParticleEffect = LoadParticleEffect(m_Name);

            if (m_ParticleEffect != null)
                m_ParticleEffect.Initialise();
        }

        public override void Load()
        {
            base.Load();

            if (m_Content != null)
            {
                foreach (AbstractEmitter emitter in m_ParticleEffect.Emitters)
                {
                    if (emitter.ParticleTextureAssetPath != null)
                    {
                        String textureDirectory = (m_BaseDirectory + "Textures/");

                        // Modify the asset path name to match what we use here
                        emitter.ParticleTextureAssetPath = emitter.ParticleTextureAssetPath.Insert(0, textureDirectory);
                    }

                    // Now load the content appropiately
                    emitter.LoadContent(m_Content);
                }
            }        
        }

        public override void UnLoad()
        {
            base.UnLoad();

           /* if ( m_ParticleEffect != null )
                m_ParticleEffect.d*/

            // Clean up particle effects
        }

        public override bool Trigger(Vector3 pos)
        {
            bool toRet = base.Trigger(pos);

            if (toRet)
            {
                Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

                if (camera != null)
                {
                    var frustum = camera.Frustum;

                    // Trigger particle
                    m_ParticleEffect.Trigger(ref pos, ref frustum);
                }
            }

            return toRet;
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            if (ParticleSystem != null)
            {
                if (ParticleSystem.ContainsActiveParticle(this))
                {
                    if (m_ParticleEffect != null)
                    {
                        // "Deltatime" ie, time since last update call
                        float SecondsPassed = (float)deltaTime.ElapsedGameTime.TotalSeconds;
                        m_ParticleEffect.Update(SecondsPassed);
                    }
                }
            }
        }

        public override void Render(GameTime deltaTime)
        {
            if (ParticleSystem != null)
            {
                if (ParticleSystem.ContainsActiveParticle(this))
                {
                    if (m_Renderer != null)
                    {
                        Camera3D camera = (CameraManager.Instance.ActiveCamera as Camera3D);

                        if (camera != null)
                        {
                            Matrix worldMatrix = Matrix.Identity;
                            Matrix viewMatrix = camera.View;
                            Matrix projectionMatrix = camera.Projection;
                            Vector3 cameraPosition = camera.Position;

                            m_Renderer.RenderEffect(m_ParticleEffect, ref worldMatrix, ref viewMatrix, ref projectionMatrix, ref cameraPosition);
                        }
                    }
                }
            }
        }

        #region Particle Loading Functions
        private ParticleEffect LoadParticleEffect(String effectName)
        {
            ParticleEffect toRet = null;

            if (m_Content != null)
            {
                try
                {
                    String particleEffectDirectory = (m_BaseDirectory + "Effects/" + effectName);
                    ParticleEffect particleEffect = m_Content.Load<ParticleEffect>(particleEffectDirectory);

                    // Deep copy the effect
                    toRet = particleEffect.DeepCopy();
                }
                catch (Exception e)
                {
                    Console.WriteLine("[ERROR] - " + e.ToString());
                }
            }

            return toRet;
        }

        private Texture2D LoadParticleTexture(String textureName)
        {
            Texture2D toRet = null;

            if (m_Content != null)
            {
                try
                {
                    String textureDirectory = (m_BaseDirectory + "Textures/" + textureName);
                    toRet = m_Content.Load<Texture2D>(textureDirectory);
                }
                catch (Exception e)
                {
                    Console.WriteLine("[ERROR] - " + e.ToString());
                }
            }

            return toRet;
        }
        #endregion
    }
}
