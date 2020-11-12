using Microsoft.Xna.Framework;
using pigeon.gameobject;

namespace pigeon.component {
    public class BoundaryExpirator : Component {
        public float ExpWindow; // how long an object needs to be off screen before it is expired
        public Rectangle Boundaries;

        private float timer;

        protected override void Initialize() { }

        protected override void Update() {
            if (Object.WorldPosition.X < Boundaries.X || Object.WorldPosition.X >= Boundaries.X + Boundaries.Width
                || Object.WorldPosition.Y < Boundaries.Y || Object.WorldPosition.Y >= Boundaries.Y + Boundaries.Height) {
                timer += time.Time.SecScaled;
                if (timer >= ExpWindow) {
                    Object.Deleted = true;
                }
            } else if (timer > 0) { // if timer > 0 then the object was off-screen but has come back on-screen, so reset the timer
                timer = 0f;
            }
        }
    }
}
