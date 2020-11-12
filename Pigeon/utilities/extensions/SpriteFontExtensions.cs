using Microsoft.Xna.Framework.Graphics;

namespace pigeon.utilities.extensions {
    public static class SpriteFontExtensions {
        public static int MeasureWidth(this SpriteFont font, string text) {
            return (int) font.MeasureString(text).X;
        }

        public static int MeasureHeight(this SpriteFont font, string text) {
            return (int) font.MeasureString(text).Y;
        }
    }
}
