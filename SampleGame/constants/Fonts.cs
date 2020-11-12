using Microsoft.Xna.Framework.Graphics;
using pigeon.data;

namespace SampleGame.constants {
    public static class Fonts {
        public static SpriteFont Numbers { get { return ResourceCache.Font("numbers"); } }
        public static SpriteFont Console { get { return ResourceCache.Font("console"); } }
        public static SpriteFont Dialog { get { return ResourceCache.Font("dialog"); } }
        public static SpriteFont TwoTone { get { return ResourceCache.Font("twotone"); } }
    }
}