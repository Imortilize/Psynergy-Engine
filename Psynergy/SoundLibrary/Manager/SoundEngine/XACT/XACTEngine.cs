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
    public class XACTEngine : SoundEngine
    {
        private String m_RootDirectory = "Content/Sounds/";

        private AudioEngine m_AudioEngine = null;
        private WaveBank m_WaveBank = null;
        private SoundBank m_SoundBank = null;

        /* Sound Effects */
        private List<Cue> m_SoundEffects = new List<Cue>();
        private List<Cue> m_SoundEffectsToRemove = new List<Cue>();
        private CustomAudioCategory m_SoundCategory;

        /* Music */
        private Cue m_CurrentSong = null;
        private CustomAudioCategory m_MusicCategory;

        public XACTEngine(ContentManager contentManager) : base(contentManager)
        {
        }

        public override void Initialise()
        {
            Reset();
        }

        public override void Load()
        {
            try
            {
                m_AudioEngine = new AudioEngine(m_RootDirectory + "sounds.xgs");

                if (m_AudioEngine != null)
                {
                    m_WaveBank = new WaveBank(m_AudioEngine, (m_RootDirectory + "Wave Bank.xwb"));
                    m_SoundBank = new SoundBank(m_AudioEngine, (m_RootDirectory + "Sound Bank.xsb"));

                    // Get sound categories
                    AudioCategory soundCategory = m_AudioEngine.GetCategory("Sounds");
                    AudioCategory musicCategory = m_AudioEngine.GetCategory("Music");

                    // Set into the custom categories
                    m_SoundCategory = new CustomAudioCategory(soundCategory);
                    m_MusicCategory = new CustomAudioCategory(musicCategory);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        
        public override void UnLoad()
        {
            if (m_SoundBank != null)
                m_SoundBank.Dispose();

            if (m_WaveBank != null)
                m_WaveBank.Dispose();

            if (m_AudioEngine != null)
                m_AudioEngine.Dispose();
        }

        public override void Reset()
        {
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Update audio engine
            if (m_AudioEngine != null)
                m_AudioEngine.Update();

            // Check if any sounds need to be removed
            for (int i = 0; i < m_SoundEffects.Count; i++)
            {
                Cue cue = m_SoundEffects[i];

                if (cue != null)
                {
                    // Sound has stopped so remove it from the play list
                    if (cue.IsStopped)
                        m_SoundEffectsToRemove.Add(cue);
                }
            }

            // Remove all detected sounds that have stopped
            foreach (Cue cue in m_SoundEffectsToRemove)
            {
                if (m_SoundEffects.Contains(cue))
                    m_SoundEffects.Remove(cue);
            }

            // Clear remove list
            m_SoundEffectsToRemove.Clear();
        }

        public override void PlaySound(String sound)
        {
            // Play sound
            if (m_SoundBank != null)
            {
                Cue cue = m_SoundBank.GetCue(sound);

                if (cue != null)
                {
                    // Add to list of playing sound effects
                    m_SoundEffects.Add(cue);

                    // Play sound 
                    cue.Play();
                }
            }
        }

        public override void PlaySound(String sound, bool loop)
        {
            PlaySound(sound);
        }

        public override void PlayMusic(String song)
        {
            if (song != "")
            {
                // Play sound
                if (m_SoundBank != null)
                {
                    // First check if a song already exists
                    if (m_CurrentSong != null)
                    {
                        // Leave function is the song being requested is already playing
                        if (m_CurrentSong.Name == song)
                        {
                            // Make sure it is playing
                            //if (!m_CurrentSong.IsPlaying)
                                //m_CurrentSong.Play();

                            return;
                        }
                    }

                    // Otherwise play next song
                    Cue cue = m_SoundBank.GetCue(song);

                    if (cue != null)
                    {
                        // Add to list of playing sound effects
                        m_CurrentSong = cue;

                        // Play sound 
                        cue.Play();
                    }
                }
            }
            else
                StopMusic();
        }

        public override void PlayMusic(String songName, bool loop)
        {
            PlayMusic(songName);
        }

        public override void Pause()
        {
            if (m_SoundCategory != null)
                m_SoundCategory.Pause();

            if (m_MusicCategory != null)
                m_MusicCategory.Pause();
        }

        public override void Resume()
        {
            if (m_SoundCategory != null)
                m_SoundCategory.Resume();

            if (m_MusicCategory != null)
                m_MusicCategory.Resume();
        }

        public override void StopSoundEffects()
        {
            if (m_SoundCategory != null)
                m_SoundCategory.Stop(AudioStopOptions.Immediate);
        }

        public override void StopMusic()
        {
            if (m_MusicCategory != null)
                m_MusicCategory.Stop(AudioStopOptions.Immediate);

            if (m_CurrentSong != null)
            {
                if (!m_CurrentSong.IsStopped)
                    m_CurrentSong.Stop(AudioStopOptions.Immediate);

                m_CurrentSong = null;
            }
        }

        public override void SetVolume(float volume)
        {
            // Set sound effects volumes
            SetSoundEffectsVolumes(volume);

            // Set music volume
            SetMusicVolume(volume);
        }

        public override void SetSoundEffectsVolumes(float volume)
        {
            if (m_SoundCategory != null)
                m_SoundCategory.SetVolume(volume);
        }

        public override void SetMusicVolume(float volume)
        {
            if (m_MusicCategory != null)
                m_MusicCategory.SetVolume(volume);
        }

        public override void Mute()
        {
            // Mute all sounds
            MuteSoundEffects();

            // Mute music
            MuteMusic();
        }

        public override void MuteSoundEffects()
        {
            if (m_SoundCategory != null)
                m_SoundCategory.Mute();

            m_MuteSoundEffects = true;
        }

        public override void MuteMusic()
        {
            if (m_MusicCategory != null)
                m_MusicCategory.Mute();

            m_MuteMusic = true;
        }

        public override void UnMute()
        {
            // Mute all sounds
            UnMuteSoundEffects();

            // Mute music
            UnMuteMusic();
        }

        public override void UnMuteSoundEffects()
        {
            if (m_SoundCategory != null)
                m_SoundCategory.UnMute();

            m_MuteSoundEffects = false;
        }

        public override void UnMuteMusic()
        {
            if (m_MusicCategory != null)
                m_MusicCategory.UnMute();

            m_MuteMusic = false;
        }

        public override bool IsMuted()
        {
            return (m_SoundCategory.IsMuted() && m_MusicCategory.IsMuted());
        }
    }
}
