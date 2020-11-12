using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pigeon.data;
using pigeon.gfx;

namespace pigeon.debugger {
    public static class DebugHelper {
        private static Texture2D debugTexture;
        public static readonly Color Red = new Color(1f, .1f, .1f, 1f);
        public static readonly Color Green = new Color(.1f, 1f, .1f, 1f);
        public static readonly Color Blue = new Color(.1f, .1f, 1f, 1f);

        public static void Initialize() {
            debugTexture = ResourceCache.Texture("pixel");
        }

        public static void DrawDot(Point position, Color color) {
            Renderer.SpriteBatch.Draw(debugTexture, position.ToVector2(), null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }
    }
}