using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NVorbis;

namespace pigeonEngine.sound {
	class OggSong : IDisposable {
		private readonly VorbisReader reader;
		private readonly DynamicSoundEffectInstance sound;

		private Thread thread;
		private readonly EventWaitHandle threadRunHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
		private readonly EventWaitHandle needBufferHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
		private readonly byte[] buffer;
		private readonly float[] nvBuffer;

		public SoundState State {
			get { return sound.State; }
		}

		public float Volume {
			get { return sound.Volume; }
			set { sound.Volume = MathHelper.Clamp(value, 0, 1); }
		}

		public bool IsLooped { get; set; }

		public float Pitch {
			set { sound.Pitch = value; }
		}

		public bool IsDisposed { get { return sound.IsDisposed; } }

		public OggSong(string oggFile) {
			reader = new VorbisReader(oggFile);
			sound = new DynamicSoundEffectInstance(reader.SampleRate, (AudioChannels) reader.Channels);
			buffer = new byte[sound.GetSampleSizeInBytes(TimeSpan.FromMilliseconds(500))];
			nvBuffer = new float[buffer.Length / 2];

			// when a buffer is needed, set our handle so the helper thread will read in more data
			sound.BufferNeeded += (s, e) => needBufferHandle.Set();
		}

		~OggSong() {
			Dispose(false);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool isDisposing) {
			threadRunHandle.Set();
			if (sound != null) {
				sound.Dispose();
			}
		}

		public void Play() {
			Stop();

			lock (sound) {
				sound.Play();
			}

			StartThread();
		}

		public void Pause() {
			lock (sound) {
				sound.Pause();
			}
		}

		public void Resume() {
			lock (sound) {
				sound.Resume();
			}
		}

		public void Stop() {
			lock (sound) {
				if (!sound.IsDisposed) {
					sound.Stop();
				}
			}

			reader.DecodedTime = TimeSpan.Zero;

			if (thread != null) {
				// set the handle to stop our thread
				threadRunHandle.Set();
				thread = null;
			}
		}

		private void StartThread() {
			if (thread == null) {
				thread = new Thread(StreamThread) { IsBackground = true };
				thread.Start();
			} else {
				throw new Exception("thread not cleaned up");
			}
		}

		private void StreamThread() {
			while (!sound.IsDisposed) {
				// sleep until we need a buffer
				while (!sound.IsDisposed && !threadRunHandle.WaitOne(0) && !needBufferHandle.WaitOne(0)) {
					Thread.Sleep(50);
				}

				// if the thread is waiting to exit, leave
				if (threadRunHandle.WaitOne(0)) {
					break;
				}

				lock (sound) {
					// ensure the effect isn't disposed
					if (sound.IsDisposed) { break; }
				}

				// read the next chunk of data
				int samplesRead = reader.ReadSamples(nvBuffer, 0, nvBuffer.Length);

				// out of data and looping? reset the reader and read again
				if (samplesRead == 0 && IsLooped) {
					reader.DecodedTime = TimeSpan.Zero;
					samplesRead = reader.ReadSamples(nvBuffer, 0, nvBuffer.Length);
				}

				if (samplesRead > 0) {
					for (int i = 0; i < samplesRead; i++) {
						short sValue = (short) Math.Max(Math.Min(short.MaxValue * nvBuffer[i], short.MaxValue), short.MinValue);
						buffer[i * 2] = (byte) (sValue & 0xff);
						buffer[i * 2 + 1] = (byte) ((sValue >> 8) & 0xff);
					}

					// submit our buffers
					lock (sound) {
						// ensure the effect isn't disposed
						if (sound.IsDisposed) { break; }
						
						sound.SubmitBuffer(buffer, 0, samplesRead);
						sound.SubmitBuffer(buffer, samplesRead, samplesRead);
					}
				}

				// reset our handle
				needBufferHandle.Reset();
			}
		}
	}
}