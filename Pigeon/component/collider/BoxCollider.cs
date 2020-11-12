using Microsoft.Xna.Framework;
using pigeon.utilities.extensions;

namespace pigeon.collision {
    public class BoxCollider : ColliderComponent {
        private readonly HitboxSetManager hitboxSetManager;
        private readonly Color color = new Color(255, 0, 0, 168);

        private bool forceActive = false;

        public BoxCollider(string hitboxName, int initialHitbox = 0, bool startFlippedX = false, bool startFlippedY = false) {
            hitboxSetManager = HitboxSetManager.Clone(hitboxName);
            hitboxSetManager.UseHitbox(initialHitbox, startFlippedX, startFlippedY);
        }

        public override void Draw() {
            GetWorldRectangle().DrawFilled(color);
        }

        public void UseHitbox(int hitboxNumber) {
            hitboxSetManager.UseHitbox(hitboxNumber, Object.IsFlippedX(), Object.IsFlippedY());
        }

        public override HitboxShapes GetShape() {
            return HitboxShapes.Box;
        }

        public override bool IsPassive() {
            return !forceActive && hitboxSetManager.IsPassive();
        }

        public void SetForceActive(bool val) {
            forceActive = val;
        }

        public override Rectangle GetRectangle() {
            return hitboxSetManager.GetHitbox();
        }

        public override void OnFlipped() {
            hitboxSetManager.SetFlipping(Object.IsFlippedX(), Object.IsFlippedY());
        }
    }
}
