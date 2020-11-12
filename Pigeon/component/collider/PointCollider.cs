using System;
using Microsoft.Xna.Framework;

namespace pigeon.collision {
    public class PointCollider : ColliderComponent {
        private readonly bool isPassive;

        private static readonly Rectangle rectangle = new Rectangle(0, 0, 1, 1);

        public PointCollider(bool isPassive = false) {
            this.isPassive = isPassive;
        }

        public override HitboxShapes GetShape() {
            return HitboxShapes.Point;
        }

        public override bool IsPassive() {
            return isPassive;
        }

        public override Rectangle GetRectangle() {
            return rectangle;
        }

        public override void OnFlipped() {
            throw new NotImplementedException("point colliders not yet flippable");
        }

        public override void Draw() {
        }
    }
}
