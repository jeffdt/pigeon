using Microsoft.Xna.Framework;

namespace pigeon.utilities.extensions {
    public static class ColorExtensions {
        public static Color WithAlpha(this Color color, byte alpha) {
            return new Color(color.R, color.G, color.B, alpha);
        }

        public static Vector3 ToVector3(this Color color) {
            return new Vector3((float) color.R / 255, (float) color.G / 255, (float) color.B / 255);
        }
    }
}
