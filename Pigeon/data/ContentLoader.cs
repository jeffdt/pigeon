using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace pigeon.data {
    public static class ContentLoader {
        public static T Load<T>(string fullPath) {
            return loadResource<T>(fullPath);
        }

        public static SpriteFont LoadFont(string path) {
            return loadResource<SpriteFont>(@"fonts\" + path);
        }

        public static Texture2D LoadTexture(string path) {
            return loadResource<Texture2D>(@"textures\" + path);
        }

        public static SoundEffect LoadSoundEffect(string path) {
            return loadResource<SoundEffect>(@"sfx\" + path);
        }

        public static Effect LoadEffect(string path) {
            return loadResource<Effect>(@"effects\" + path);
        }

        public static Song LoadSong(string path) {
            return loadResource<Song>(@"music\" + path);
        }

        private static T loadResource<T>(string path) {
            return Pigeon.ContentManager.Load<T>(path);
        }
    }
}
