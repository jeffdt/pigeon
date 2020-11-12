using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace pigeon.gfx.drawable {
    public abstract class Graphic {
        public Point Size = new Point(1);
        public Vector2 Parallax = new Vector2(1);

        public Color Color = Color.White;
        private float rotation = 0;
        public Vector2 Scale = Vector2.One;

        internal Vector2 InternalOffset;

        public Point Offset {
            get { return InternalOffset.ToPoint(); }
            set { InternalOffset = value.ToVector2(); }
        }

        public SpriteEffects Flip = SpriteEffects.None;

        public float Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
                if (rotation >= MathHelper.TwoPi) {
                    rotation -= MathHelper.TwoPi;
                }
            }
        }

        public abstract void Update();
        public abstract void Draw(Vector2 position, float layer);
    }
}
