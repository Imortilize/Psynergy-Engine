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
    class CustomAudioCategory
    {
        private AudioCategory m_AudioCategory;

        // Volume on mute
        private bool m_Muted = false;
        private float m_VolumeBeforeMute = 1.0f;

        public CustomAudioCategory(AudioCategory audioCategory)
        {
            m_AudioCategory = audioCategory;
        }

        public void Reset()
        {
            if (m_AudioCategory != null)
                m_AudioCategory.SetVolume( 1.0f );
        }

        public void SetVolume(float volume)
        {
            if (m_AudioCategory != null)
            {
                if ( !m_Muted )
                    m_AudioCategory.SetVolume(volume);
                
                // Set volume before muting
                m_VolumeBeforeMute = volume;
            }
        }

        public void Mute()
        {
            // Mute sound instance
            if (m_AudioCategory != null)
                m_AudioCategory.SetVolume(0.0f);

            m_Muted = true;
        }

        public void UnMute()
        {
            if (m_AudioCategory != null)
                m_AudioCategory.SetVolume(m_VolumeBeforeMute);

            m_Muted = false;
        }

        public void Pause()
        {
            if (m_AudioCategory != null)
                m_AudioCategory.Pause();
        }

        public void Resume()
        {
            if (m_AudioCategory != null)
                m_AudioCategory.Resume();
        }

        public void Stop( AudioStopOptions options )
        {
            if (m_AudioCategory != null)
                m_AudioCategory.Stop(options);
        }

        public bool IsMuted()
        {
            return m_Muted;
        }

        #region Property Set / Gets
        public AudioCategory AudioCategory { get { return m_AudioCategory; } }
        #endregion
    }
}
