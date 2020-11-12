using Microsoft.Xna.Framework;

namespace pigeon.collision {
    internal class HitboxSet {
        public bool IsPassive;
        // internal string Name;
        private readonly Rectangle[] hitboxes = new Rectangle[4];

        public Rectangle GetHitbox(bool isFlippedX, bool isFlippedY) {
            int index = 0;  // default orientation

            if (isFlippedX && !isFlippedY) {    // flipped horizontally
                index = 1;
            } else if (!isFlippedX && isFlippedY) {     // flipped vertically
                index = 2;
            } else if (isFlippedX && isFlippedY) {  // flipped both
                index = 3;
            }

            return hitboxes[index];
        }

        internal HitboxSet(Point position, Point dimensions, bool hFlip, bool vFlip, bool passive) {
            var originalHitbox = new Rectangle {
                X = position.X,
                Y = position.Y,
                Width = dimensions.X,
                Height = dimensions.Y
            };

            hitboxes[0] = originalHitbox;
            hitboxes[1] = hFlip ? calcFlippedHitbox(originalHitbox, true, false) : originalHitbox;
            hitboxes[2] = vFlip ? calcFlippedHitbox(originalHitbox, false, true) : originalHitbox;
            hitboxes[3] = hFlip && vFlip ? calcFlippedHitbox(originalHitbox, true, true) : originalHitbox;

            IsPassive = passive;
        }

        private static Rectangle calcFlippedHitbox(Rectangle originalHitbox, bool flipX, bool flipY) {
            int x = originalHitbox.X;
            int y = originalHitbox.Y;

            if (flipX) {
                x = -x - (originalHitbox.Width - 1);
            }

            if (flipY) {
                y = -y - (originalHitbox.Height - 1);
            }

            return new Rectangle {
                Width = originalHitbox.Width,
                Height = originalHitbox.Height,
                X = x,
                Y = y
            };
        }
    }
}
