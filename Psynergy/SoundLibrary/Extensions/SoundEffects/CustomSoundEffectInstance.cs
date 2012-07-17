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
    class CustomSoundEffectInstance
    {
        private SoundEffectInstance m_SoundInstance = null;

        // Volume on mute
        private bool m_Muted = false;
        private float m_VolumeBeforeMute = 1.0f;

        public CustomSoundEffectInstance(SoundEffectInstance soundInstance)
        {
            m_SoundInstance = soundInstance;
        }

        public void Reset()
        {
            if (m_SoundInstance != null)
            {
                m_SoundInstance.Volume = 1.0f;
            }
        }

        public void Play()
        {
            if (m_SoundInstance != null)
                m_SoundInstance.Play();
        }

        public void SetVolume(float volume)
        {
            if (m_SoundInstance != null)
            {
                if ( !m_Muted )
                    m_SoundInstance.Volume = volume;
                else
                    m_VolumeBeforeMute = volume;
            }
        }

        public void SetPitch(float pitch)
        {
            if (m_SoundInstance != null)
                m_SoundInstance.Pitch = pitch;
        }

        public void SetPan(float pan)
        {
            if (m_SoundInstance != null)
                m_SoundInstance.Pan = pan;
        }

        public void Mute()
        {
            if (m_SoundInstance != null)
            {
                m_VolumeBeforeMute = m_SoundInstance.Volume;

                // Mute sound instance
                m_SoundInstance.Volume = 0.0f;
            }

            m_Muted = true;
        }

        public void UnMute()
        {
            if (m_SoundInstance != null)
            {
                // Restore to former sound volume
                m_SoundInstance.Volume = m_VolumeBeforeMute;
            }

            m_Muted = false;
        }

        public void SetLoop(bool loop)
        {
            if (m_SoundInstance != null)
                m_SoundInstance.IsLooped = loop;
        }

        public bool IsLooped()
        {
            bool toRet = false;

            if (m_SoundInstance != null)
                toRet = m_SoundInstance.IsLooped;

            return toRet;
        }

        public void Pause()
        {
            if (m_SoundInstance != null)
                m_SoundInstance.Pause();
        }

        public void Resume()
        {
            if (m_SoundInstance != null)
                m_SoundInstance.Resume();
        }

        public void Stop()
        {
            if (m_SoundInstance != null)
                m_SoundInstance.Stop();
        }

        #region Property Set / Gets
        public SoundEffectInstance SoundInstance { get { return m_SoundInstance; } }
        #endregion
    }
}
