using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using pigeon.data;
using pigeon.sound.music;

namespace pigeon.sound {
    public static class Sfx {
        public class SfxVolumeChangedEvent : EventArgs { }
        public static float GlobalPitch = 0f;
        private static readonly List<SoundEffectInstance> activeSfx = new List<SoundEffectInstance>();

        public static void Initialize() { }

        public static void Update() {
            cleanUpCompletedSfx(false);
        }

        public static void PauseAllSfx() {
            foreach (var sfx in activeSfx) {
                if (sfx.State != SoundState.Paused) {
                    sfx.Pause();
                }
            }
        }

        public static void StopAllSfx() {
            cleanUpCompletedSfx(true);
        }

        public static void ResumeAllSfx() {
            foreach (var sfx in activeSfx) {
                if (sfx.State == SoundState.Paused) {
                    sfx.Resume();
                }
            }
        }

        private static void cleanUpCompletedSfx(bool forceDisposeAll) {
            if (activeSfx.Count > 0) {
                for (int index = activeSfx.Count - 1; index >= 0; index--) {
                    var sfxInstance = activeSfx[index];
                    if (forceDisposeAll || sfxInstance.State == SoundState.Stopped) {
                        activeSfx.RemoveAt(index);
                        sfxInstance.Dispose();
                    }
                }

                if (activeSfx.Count == 0) {
                    Music.VolumeState = MusicVolumes.Full;
                }
            }
        }

        public static float SfxVolume {
            get { return SoundEffect.MasterVolume; }
            set {
                SoundEffect.MasterVolume = value;
                Pigeon.GameEventRegistry.RaiseEmptyEvent<SfxVolumeChangedEvent>();
            }
        }

        public static void PlayScaledSfx(string name) {
            PlaySfx(name, GlobalPitch);
        }

        public static void PlaySfx(string name, float volume = 1f) {
            if (name == null) {
                return;
            }

            SoundEffectInstance instance = ResourceCache.Sound(name).CreateInstance();
            instance.Play();
            instance.Volume = volume;
            activeSfx.Add(instance);

            Music.VolumeState = MusicVolumes.Dimmed;
        }
    }
}
