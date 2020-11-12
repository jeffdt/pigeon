using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pigeon.data;
using pigeon.gfx;

namespace pigeon.utilities.extensions {
    public static class RectangleExtensions {
        public static Rectangle Add(this Rectangle rect, Point vector) {
            return new Rectangle(rect.X + vector.X, rect.Y + vector.Y, rect.Width, rect.Height);
        }

        public static Point Min(this Rectangle rect) {
            return new Point(rect.X, rect.Y);
        }

        public static Point Max(this Rectangle rect) {
            return new Point(rect.Right, rect.Bottom);
        }

        public static bool Contains(this Rectangle rectangle, Vector2 vector) {
            return vector.X >= rectangle.X
                && vector.X < rectangle.X + rectangle.Width
                && vector.Y >= rectangle.Y
                && vector.Y < rectangle.Y + rectangle.Height;
        }

        public static bool Excludes(this Rectangle rectA, Rectangle rectB) {
            return rectB.Bottom < rectA.Top || rectB.Left > rectA.Right || rectB.Top > rectA.Bottom || rectB.Right < rectA.Left;
        }

        public static void DrawFilledBordered(this Rectangle rect, Color fillColor, Color borderColor, int thickness = 1) {
            DrawFilled(rect, fillColor);
            DrawBordered(rect, borderColor, thickness);
        }

        public static void DrawFilled(this Rectangle rect, Color color, float layer = 1f) {
            Renderer.SpriteBatch.Draw(ResourceCache.Pixel, rect, null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        public static void DrawFilled(this int xPosition, int yPosition, int width, int height, Color color, float layer = 1f) {
            Renderer.SpriteBatch.Draw(ResourceCache.Pixel, new Rectangle(xPosition, yPosition, width, height), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        public static void DrawBordered(this Rectangle rect, Color color, int thickness = 1, float layer = 1f) {
            DrawBordered(rect.X, rect.Y, rect.Width, rect.Height, color, thickness, layer);
        }

        public static void DrawBordered(this int xPos, int yPos, int width, int height, Color color, int thickness = 1, float layer = 1f) {
            // top
            Renderer.SpriteBatch.Draw(ResourceCache.Pixel, new Rectangle(xPos, yPos, width, thickness), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            // bottom
            Renderer.SpriteBatch.Draw(ResourceCache.Pixel, new Rectangle(xPos, yPos + height - thickness, width, thickness), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);

            // left
            Renderer.SpriteBatch.Draw(ResourceCache.Pixel, new Rectangle(xPos, yPos, thickness, height), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
            // right
            Renderer.SpriteBatch.Draw(ResourceCache.Pixel, new Rectangle(xPos + width - thickness, yPos, thickness, height), null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }
    }
}
