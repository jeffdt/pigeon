using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using pigeon.gameobject;

namespace pigeon.gfx.drawable.image {
    public class ImageRenderer : Component, IRenderable, IFlippable {
        public string TexturePath;
        public bool Center;

        public Image Image;

        public Point InitialAnchor;
        public Color InitialColor = Color.White;
        private readonly float initialAlpha = -1;

        protected Texture2D initialTexture;

        public ImageRenderer(string texturePath, bool center = false, float initialAlpha = -1) {
            TexturePath = texturePath;
            Center = center;
            this.initialAlpha = initialAlpha;
        }

        public ImageRenderer(string texturePath, Point anchor, float initialAlpha = -1) {
            TexturePath = texturePath;
            InitialAnchor = anchor;
            this.initialAlpha = initialAlpha;
        }

        public ImageRenderer() { }   // can leave configuration up to custom components

        protected override void Initialize() {
            if (Image != null) {
                return;
            }

            if (initialTexture != null) {   // build image from custom texture
                Image = Image.Create(initialTexture);
                Image.Offset = InitialAnchor;
            } else {    // build image from standard named texture
                if (InitialAnchor != Point.Zero) {
                    Image = Image.Create(TexturePath);
                    Image.Offset = InitialAnchor;
                } else {
                    Image = Image.Create(TexturePath, Center);
                }
            }

            Image.Color = InitialColor;

            if (initialAlpha != -1) {
                SetAlpha(initialAlpha);
            }
        }

        public void SetAlpha(float a) {
            if (a < 0f || a > 1f) {
                return;
            }

            Image.Color.A = (byte) (255f * a);
        }

        protected override void Update() {
        }

        public void Draw() {
            if (Enabled) {
                Image.Draw(Object.WorldPosition.ToVector2(), Object.DrawLayer);
            }
        }

        public void OnFlipped() {
            SetFlipHorizontal(Object.IsFlippedX());
            SetFlipVertical(Object.IsFlippedY());
        }

        public bool IsFlippedHorizontal() {
            return (Image.Flip & SpriteEffects.FlipHorizontally) != 0;
        }

        public bool IsFlippedVertical() {
            return (Image.Flip & SpriteEffects.FlipVertically) != 0;
        }

        public void SetFlipHorizontal(bool hFlip) {
            if (hFlip) {
                Image.Flip |= SpriteEffects.FlipHorizontally;
            } else {
                Image.Flip &= SpriteEffects.FlipVertically;
            }
        }

        public void SetFlipVertical(bool vFlip) {
            if (vFlip) {
                Image.Flip |= SpriteEffects.FlipVertically;
            } else {
                Image.Flip &= SpriteEffects.FlipHorizontally;
            }
        }
    }
}
