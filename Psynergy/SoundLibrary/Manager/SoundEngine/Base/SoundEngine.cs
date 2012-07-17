using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace Psynergy.Sound
{
    public class SoundEngine
    {
        protected ContentManager m_ContentManager = null;

        // Muting
        protected bool m_MuteSoundEffects = false;
        protected bool m_MuteMusic = false;

        public SoundEngine(ContentManager contentManager) : base()
        {
            m_ContentManager = contentManager;
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

        public virtual void Reset()
        {
        }

        public virtual void Update(GameTime deltaTime)
        {
        }

        public virtual SoundEffect LoadSoundEffect(String sound)
        {
            return null;
        }

        public virtual void PlaySound(String sound)
        {
        }

        public virtual void PlaySound(String sound, bool loop)
        {
        }

        /* Music */
        public virtual Song LoadSong(String songDir)
        {
            return null;
        }

        public virtual void PlayMusic(String song)
        {
        }

        public virtual void PlayMusic(String songName, bool loop)
        {
        }
        /**/

        public virtual void SetVolume(float volume)
        {
        }

        public virtual void SetSoundEffectsVolumes(float volume)
        {
        }

        public virtual void SetMusicVolume(float volume)
        {
        }

        public virtual void Mute()
        {
        }

        public virtual void MuteSoundEffects()
        {
        }

        public virtual void MuteMusic()
        {
        }

        public virtual void UnMute()
        {
        }

        public virtual void UnMuteSoundEffects()
        {
        }

        public virtual void UnMuteMusic()
        {
        }

        public virtual void Pause()
        {
        }

        public virtual void Resume()
        {
        }

        public void Stop()
        {
            StopSoundEffects();
            StopMusic();
        }

        public virtual void StopSoundEffects()
        {
        }

        public virtual void StopMusic()
        {
        }

        public virtual bool IsMuted()
        {
            return false;
        }
    }
}
