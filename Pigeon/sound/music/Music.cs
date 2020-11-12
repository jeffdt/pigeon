using Microsoft.Xna.Framework;
using pigeon.time;

namespace pigeon.sound.music {
    public static class Music {
        public const int P1 = 0;
        public const int P2 = 1;
        public const int TRI = 2;
        public const int NOISE = 3;
        public const int DPCM = 4;
        public const int SAW = 5;
        public const int P3 = 6;
        public const int P4 = 7;

        public static float DimmedVolume { get; set; } = 0.6f;
        public static float FullVolume { get; set; } = 0.6f;
        public static float VolumeRampupTimeSecs { get; set; } = .75f;

        private static bool transitioning = false;
        private static MusicVolumes musicVolume = MusicVolumes.Full;
        private static float volumeTransitionTimer;

        private static MusicPlayer musicPlayer = new MusicPlayer();

        public static void Initialize() {
            musicPlayer.Initialize();
        }

        public static void Load(string filename) {
            musicPlayer.Load(filename);
            setFullVolume();
        }

        internal static void Update() {
            if (!transitioning) {
                return;
            }

            if (musicVolume == MusicVolumes.Full && musicPlayer.Volume < FullVolume - float.Epsilon) {
                volumeTransitionTimer -= Time.SecScaled;
                float lerped = MathHelper.Lerp(DimmedVolume, FullVolume, (VolumeRampupTimeSecs - volumeTransitionTimer) / VolumeRampupTimeSecs);

                if (lerped >= FullVolume - float.Epsilon) {
                    setFullVolume();
                } else {
                    musicPlayer.Volume = lerped;
                }
            }
        }

        private static void setFullVolume() {
            musicPlayer.Volume = FullVolume;
            transitioning = false;
        }

        #region playback controls
        public static void PlayTrack(int trackNum) {
            musicPlayer.PlayTrack(trackNum);
        }

        public static void Play() {
            musicPlayer.Play();
            musicPlayer.StereoDepth = .4f;
        }

        public static void Stop() {
            musicPlayer.Stop();
        }

        public static void Pause() {
            musicPlayer.Pause();
        }
        #endregion

        #region volume controls
        public static MusicVolumes VolumeState {
            set {
                switch (value) {
                    case MusicVolumes.Full:
                        transitioning = true;
                        musicVolume = MusicVolumes.Full;
                        volumeTransitionTimer = VolumeRampupTimeSecs;
                        break;
                    case MusicVolumes.Dimmed:
                        transitioning = false;
                        musicVolume = MusicVolumes.Dimmed;
                        musicPlayer.Volume = DimmedVolume;
                        break;
                    case MusicVolumes.Silent:
                        transitioning = false;
                        musicVolume = MusicVolumes.Silent;
                        musicPlayer.Volume = 0;
                        break;
                }
            }
        }

        public static void SetVolumeInstant(float value) {
            musicPlayer.Volume = value;
        }

        public static float Volume { get { return musicPlayer.Volume; } }
        #endregion

        #region voice muting
        public static void SetVoiceMute(int voiceIndex, bool mute) {
            musicPlayer.SetVoiceMute(voiceIndex, mute);
        }

        public static void MuteVoice(int voiceIndex) {
            musicPlayer.SetVoiceMute(voiceIndex, true);
        }

        public static void UnmuteVoice(int voiceIndex) {
            musicPlayer.SetVoiceMute(voiceIndex, false);
        }

        public static void MuteVoices(params int[] voiceIndexes) {
            foreach (int voiceIndex in voiceIndexes) {
                MuteVoice(voiceIndex);
            }
        }

        public static void UnmuteVoices(params int[] voiceIndexes) {
            foreach (int voiceIndex in voiceIndexes) {
                UnmuteVoice(voiceIndex);
            }
        }

        public static void MaskMuteVoices(int mutingMask) {
            musicPlayer.MaskMuteVoices(mutingMask);
        }
        #endregion

        #region effects
        public static double Treble {
            get { return musicPlayer.Treble; }
            set { musicPlayer.Treble = value; }
        }

        public static double Bass {
            get { return musicPlayer.Bass; }
            set { musicPlayer.Bass = value;
            }
        }

        public static double StereoDepth {
            get { return musicPlayer.StereoDepth; }
            set {
                musicPlayer.StereoDepth = value;
            }
        }

        public static int Fade {
            get { return musicPlayer.Fade; }
            set { musicPlayer.Fade = value;}
        }

        public static double Tempo {
            get { return musicPlayer.Tempo; }
            set { musicPlayer.Tempo = value; }
        }
        #endregion
    }
}
