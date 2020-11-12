using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pigeon.data;
using pigeon.gfx;

namespace pigeon.gfx.drawable.image {
    public sealed class Image : Graphic {
        public Texture2D Texture;
        public Rectangle SourceRect;

        private Dictionary<object, Rectangle> sourceMap;

        private Image() { }

        public static Image Create(Texture2D texture, bool center = false) {
            return create(texture, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, center);
        }

        public static Image Create(Texture2D texture, Rectangle sourceRect, bool center = false) {
            return create(texture, sourceRect, Color.White, center);
        }

        public static Image Create(string texturePath, bool center = false) {
            Texture2D texture = ResourceCache.Texture(texturePath);
            return create(texture, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, center);
        }

        public static Image Create(string texturePath, Color tint, bool center = false) {
            Texture2D texture = ResourceCache.Texture(texturePath);
            return create(texture, new Rectangle(0, 0, texture.Width, texture.Height), tint, center);
        }

        public static Image Create(string texturePath, Rectangle sourceRect, bool center = false) {
            return create(ResourceCache.Texture(texturePath), sourceRect, Color.White, center);
        }

        public static Image Create(string texturePath, Rectangle sourceRect, Point offset) {
            Image image = create(ResourceCache.Texture(texturePath), sourceRect, Color.White, false);
            image.Offset = offset;
            return image;
        }

        private static Image create(Texture2D texture, Rectangle sourceRect, Color color, bool center) {
            Image image = new Image {
                Texture = texture,
                SourceRect = sourceRect,
                Size = new Point(sourceRect.Width, sourceRect.Height),
                Color = color
            };

            if (center) {
                centerImage(image);
            }

            return image;
        }

        private static void centerImage(Image image) {
            image.Offset = new Point(image.SourceRect.Width / 2, image.SourceRect.Height / 2);
        }

        public Image Clone() {
            return clone(Texture);
        }

        public Image Clone(string texturePath) {
            return clone(ResourceCache.Texture(texturePath));
        }

        private Image clone(Texture2D texture) {
            return new Image { Texture = texture, SourceRect = SourceRect, sourceMap = sourceMap, Size = Size, Offset = Offset, Color = Color };
        }

        public void AddSource(object key, Rectangle source) {
            (sourceMap ?? (sourceMap = new Dictionary<object, Rectangle>())).Add(key, source);
        }

        public void SwapSource(object key, bool center = false) {
            if (sourceMap != null && sourceMap.TryGetValue(key, out Rectangle value)) {
                SourceRect = value;
                if (center) {
                    centerImage(this);
                }
            } else {
                Pigeon.Console.Error("no source on image with key: " + key);
            }
        }

        public override void Update() { }

        public override void Draw(Vector2 position, float layer) {
            Renderer.SpriteBatch.Draw(Texture, position, SourceRect, Color, Rotation, Offset.ToVector2(), Scale, Flip, layer);
        }
    }
}
