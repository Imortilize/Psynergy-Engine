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
    public class DirectSoundEngine : SoundEngine
    {
        private String m_RootDirectory = "Sounds/";

        /* Sound Effects */
        private SortedList<String, SoundEffect> m_LoadedSoundEffects = new SortedList<String, SoundEffect>();
        private List<CustomSoundEffectInstance> m_SoundEffects = new List<CustomSoundEffectInstance>();
        private List<CustomSoundEffectInstance> m_SoundEffectsToRemove = new List<CustomSoundEffectInstance>();

        /* Music */
        private SortedList<String, Song> m_LoadedSongs = new SortedList<String, Song>();
        private Song m_CurrentSong = null;

        public DirectSoundEngine(ContentManager contentManager) : base(contentManager)
        {
        }

        public override void Initialise()
        {
            Reset();
        }

        public override void Load()
        {
        }

        public override void UnLoad()
        {
        }

        public override void Reset()
        {
            m_SoundEffects.Clear();
            m_SoundEffectsToRemove.Clear();
        }

        public override void Update(GameTime deltaTime)
        {
            base.Update(deltaTime);

            // Check if any sounds need to be removed
            for (int i = 0; i < m_SoundEffects.Count; i++)
            {
                CustomSoundEffectInstance customSoundInstance = m_SoundEffects[i];
                SoundEffectInstance soundInstance = customSoundInstance.SoundInstance;

                if (soundInstance != null)
                {
                    // Sound has stopped so remove it from the play list
                    if (soundInstance.State == SoundState.Stopped)
                        m_SoundEffectsToRemove.Add(customSoundInstance);
                }
            }

            // Remove all detected sounds that have stopped
            foreach (CustomSoundEffectInstance soundInstance in m_SoundEffectsToRemove)
            {
                if (m_SoundEffects.Contains(soundInstance))
                    m_SoundEffects.Remove(soundInstance);
            }

            // Clear remove list
            m_SoundEffectsToRemove.Clear();
        }

        /* Sound Effects */
        public override SoundEffect LoadSoundEffect(String sound)
        {
            SoundEffect soundEffect = null;

            // See if the sound has already been loaded or not
            String soundKey = sound;
            int index = sound.LastIndexOf("/");

            if (index >= 0)
                soundKey = sound.Substring(index);

            if (!m_LoadedSoundEffects.ContainsKey(soundKey))
            {
                if (m_ContentManager != null)
                {
                    try
                    {
                        soundEffect = m_ContentManager.Load<SoundEffect>(m_RootDirectory + sound);
                    }
                    catch (NoAudioHardwareException e)
                    {
                        Console.WriteLine(e.ToString());
                    }

                    if (soundEffect != null)
                        m_LoadedSoundEffects.Add(soundKey, soundEffect);
                }
            }
            else
                soundEffect = m_LoadedSoundEffects.Values[index];

            return soundEffect;
        }

        public override void PlaySound(String sound)
        {
            PlaySound(sound, false);
        }

        public override void PlaySound(String sound, bool loop)
        {
            SoundEffect soundEffect = null;

            // Load sound effect ( either from content or from the sound effets buffer
            soundEffect = LoadSoundEffect(sound);

            // If the sound effect was successfully returned
            if (soundEffect != null)
            {
                CustomSoundEffectInstance soundInstance = new CustomSoundEffectInstance(soundEffect.CreateInstance());

                if (soundInstance != null)
                {
                    // Set whether to loop it or not
                    soundInstance.SetLoop(loop);

                    // Check muted flag
                    if (m_MuteSoundEffects)
                        soundInstance.Mute();

                    // Play sound
                    soundInstance.Play();
                }

                // Add to sound effect instance buffer
                m_SoundEffects.Add(soundInstance);
            }
        }

        /**/

        /* Music */
        public override Song LoadSong(String songDir)
        {
            Song song = null;

            // See if the sound has already been loaded or not
            String songKey = songDir;
            int index = songDir.LastIndexOf("/");

            if (index >= 0)
                songKey = songDir.Substring(index);

            if (!m_LoadedSongs.ContainsKey(songKey))
            {
                if (m_ContentManager != null)
                {
                    try
                    {
                        song = m_ContentManager.Load<Song>(m_RootDirectory + songDir);
                    }
                    catch (NoAudioHardwareException e)
                    {
                        Console.WriteLine(e.ToString());
                    }

                    if (song != null)
                        m_LoadedSongs.Add(songKey, song);
                }
            }
            else
                song = m_LoadedSongs.Values[index];

            return song;
        }

        public override void PlayMusic(String song)
        {
            PlayMusic(song);
        }

        public override void PlayMusic(String songName, bool loop)
        {
            Song song = null;

            // Load sound effect ( either from content or from the sound effets buffer
            song = LoadSong(songName);

            // If the sound effect was successfully returned
            if (song != null)
            {
                // If song is currently playing, stop it ( for now no fading )
                MediaPlayer.Stop();

                // Set repeating flag
                MediaPlayer.IsRepeating = loop;

                // Play new song
                MediaPlayer.Play(song);
            }
        }
        /**/

        public override void SetVolume(float volume)
        {
            // Set sound effects volumes
            SetSoundEffectsVolumes(volume);

            // Set music volume
            SetMusicVolume(volume);
        }

        public override void SetSoundEffectsVolumes(float volume)
        {
            foreach (CustomSoundEffectInstance soundInstance in m_SoundEffects)
                soundInstance.SetVolume(volume);
        }

        public override void SetMusicVolume(float volume)
        {
            MediaPlayer.Volume = volume;
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
            foreach (CustomSoundEffectInstance soundInstance in m_SoundEffects)
                soundInstance.Mute();

            m_MuteSoundEffects = true;
        }

        public override void MuteMusic()
        {
            m_MuteMusic = true;

            // Mute media player
            MediaPlayer.IsMuted = m_MuteMusic;
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
            foreach (CustomSoundEffectInstance soundInstance in m_SoundEffects)
                soundInstance.UnMute();

            m_MuteSoundEffects = false;
        }

        public override void UnMuteMusic()
        {
            m_MuteMusic = false;

            // UnMute media player
            MediaPlayer.IsMuted = m_MuteMusic;
        }

        public override bool IsMuted()
        {
            return MediaPlayer.IsMuted;
        }
    }
}
