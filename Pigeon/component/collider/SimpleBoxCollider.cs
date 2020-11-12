using Microsoft.Xna.Framework;
using pigeon.utilities.extensions;

namespace pigeon.collision {
    public class SimpleBoxCollider : ColliderComponent {
        public Rectangle Hitbox;

        public bool Passive = true;

        private readonly Color color = new Color(0, 0, 255, 168);

        public override HitboxShapes GetShape() {
            return HitboxShapes.Box;
        }

        public override bool IsPassive() {
            return Passive;
        }

        public override Rectangle GetRectangle() {
            return Hitbox;
        }

        public override void OnFlipped() { }

        public override void Draw() {
            GetWorldRectangle().DrawFilled(color);
        }
    }
}
