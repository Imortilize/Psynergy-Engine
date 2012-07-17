using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;


namespace Psynergy.Sound
{
    public enum SoundEngineType
    {
        None = 0,
        Direct = 1,
        XACT = 2
    }

    public class SoundManager : Singleton<SoundManager>
    {
        private ContentManager m_ContentManager = null;

        // Sound Engine
        private SoundEngine m_SoundEngine = null;
        private SoundEngineType m_EngineType = SoundEngineType.None;
        
        public SoundManager() : base()
        {
        }

        public SoundManager(ContentManager contentManager) : base()
        {
            m_ContentManager = contentManager;
        }

        public void SetEngineType(SoundEngineType type)
        {
            m_EngineType = type;
        }

        public override void Initialise()
        {
            base.Initialise();

            // Initialise to set sound engine type
            switch (m_EngineType)
            {
                case SoundEngineType.None:
                    {
                        // Create sound engine
                        m_SoundEngine = new SoundEngine(m_ContentManager);
                    }
                    break;
                case SoundEngineType.Direct:
                    {
                        // Create sound engine
                        m_SoundEngine = new DirectSoundEngine(m_ContentManager);
                    }
                    break;
                case SoundEngineType.XACT:
                    {
                        // Create sound engine
                        m_SoundEngine = new XACTEngine(m_ContentManager);
                    }
                    break;
            }

            // Initialise sound engine
            if (m_SoundEngine != null)
                m_SoundEngine.Initialise();
        }

        public override void Load()
        {
            base.Load();

            // Initialise sound engine
            if (m_SoundEngine != null)
                m_SoundEngine.Load();
        }

        public override void UnLoad()
        {
            base.UnLoad();

            // Initialise sound engine
            if (m_SoundEngine != null)
                m_SoundEngine.UnLoad();
        }

        public override void Reset()
        {
            base.Reset();

            // Initialise sound engine
            if (m_SoundEngine != null)
                m_SoundEngine.Reset();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            if (m_SoundEngine != null)
                m_SoundEngine.Update(deltaTime);
        }

        public SoundEffect LoadSoundEffect(String sound)
        {
            SoundEffect toRet = null;

            if (m_SoundEngine != null)
                toRet = m_SoundEngine.LoadSoundEffect(sound);

            return toRet;
        }

        public void PlaySound(String sound)
        {
            if ( m_SoundEngine != null )
                m_SoundEngine.PlaySound(sound);
        }

        public void PlaySound(String sound, bool loop)
        {
            if (m_SoundEngine != null)
                m_SoundEngine.PlaySound(sound, loop);
        }

        public void PlayMusic(String sound)
        {
            if (m_SoundEngine != null)
                m_SoundEngine.PlayMusic(sound);
        }

        public void PlayMusic(String sound, bool loop)
        {
            if (m_SoundEngine != null)
                m_SoundEngine.PlayMusic(sound, loop);
        }

        public void SetVolume(float volume)
        {
            if (m_SoundEngine != null)
                m_SoundEngine.SetVolume(volume);
        }

        public void SetSoundEffectsVolumes(float volume)
        {
            if (m_SoundEngine != null)
                m_SoundEngine.SetSoundEffectsVolumes(volume);
        }

        public void SetMusicVolumes(float volume)
        {
            if (m_SoundEngine != null)
                m_SoundEngine.SetMusicVolume(volume);
        }

        public void Mute()
        {
            if (m_SoundEngine != null)
                m_SoundEngine.Mute();
        }

        public void MuteSoundEffects()
        {
            if (m_SoundEngine != null)
                m_SoundEngine.MuteSoundEffects();
        }

        public void MuteMusic()
        {
            if (m_SoundEngine != null)
                m_SoundEngine.MuteMusic();
        }

        public void UnMute()
        {
            if (m_SoundEngine != null)
                m_SoundEngine.UnMute();
        }

        public void UnMuteSoundEffects()
        {
            if (m_SoundEngine != null)
                m_SoundEngine.UnMuteSoundEffects();
        }

        public void UnMuteMusic()
        {
            if (m_SoundEngine != null)
                m_SoundEngine.UnMuteMusic();
        }

        public void StopMusic()
        {
            if (m_SoundEngine != null)
                m_SoundEngine.StopMusic();
        }

        public bool IsMuted()
        {
            bool toRet = false;

            if (m_SoundEngine != null)
                toRet = m_SoundEngine.IsMuted();

            return toRet;
        }
    }
}
