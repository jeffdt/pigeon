using GameMusicEmuSharp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using pigeon.utilities.extensions;

namespace pigeon.sound {
    internal class MusicPlayer {
        public float Volume {
            get { return (volumeSampleProvider == null) ? 0f : volumeSampleProvider.Volume; }
            set { if (volumeSampleProvider != null) { volumeSampleProvider.Volume = value; } }
        }

        private VolumeSampleProvider volumeSampleProvider;
        private GmeReader reader;
        private IWavePlayer player;

        public void Initialize() { }

        public void Load(string filename) {
            if (player != null) {
                player.Stop();
                player.Dispose();
            }

            if (reader != null) {
                reader.Dispose();
                reader.Unload();
            }

            reader = new GmeReader(filename);

            volumeSampleProvider = new VolumeSampleProvider(reader.ToSampleProvider());

            var waveOut = new DirectSoundOut();
            waveOut.Init(volumeSampleProvider);

            player = waveOut;
        }

        #region playback controls
        public void PlayTrack(int trackNum) {
            int clampedTrackNum = trackNum.Clamp(0, reader.TrackCount - 1);

            if (trackNum < 0 || trackNum > reader.TrackCount - 1) {
                Pigeon.Console.Error(string.Format("trackNum {0} not valid for trackCount range [0, {1}]. defaulting to {2}.", trackNum, reader.TrackCount - 1, clampedTrackNum));
            }

            reader.SetTrack(clampedTrackNum);
            Play();
        }

        public void Play() {
            player.Play();
        }

        public void Stop() {
            player?.Stop();
        }

        public void Pause() {
            player.Pause();
        }
        #endregion

        #region voice muting
        public void SetVoiceMute(int voiceIndex, bool mute) {
            reader.MuteVoice(voiceIndex, mute ? 1 : 0);
        }

        public void MaskMuteVoices(int mutingMask) {
            reader.MuteVoices(mutingMask);
        }
        #endregion

        #region effect controls
        public void ResetEqualizer() {
            reader.SetEqualizer(0, 90);
        }

        private double _treble;
        private double _bass;
        private double _stereoDepth;
        private int _fade;
        private double _tempo;

        public double Treble {
            get { return _treble; }
            set {
                _treble = value.Clamp(-50, 5);
                reader.SetEqualizer(_treble, _bass);
            }
        }
        public double Bass {
            get { return _bass; }
            set {
                _bass = value.Clamp(1, 16000);
                reader.SetEqualizer(_treble, _bass);
            }
        }
        public double StereoDepth {
            get { return _stereoDepth; }
            set {
                _stereoDepth = value.Clamp(0, 1);
                reader.SetStereoDepth(_stereoDepth);
            }
        }
        public int Fade {
            get { return _fade; }
            set {
                _fade = value;
                reader.SetFade(_fade);
            }
        }
        public double Tempo {
            get { return _tempo; }
            set {
                _tempo = value;
                reader.SetTempo(_tempo);
            }
        }
        #endregion
    }
}
