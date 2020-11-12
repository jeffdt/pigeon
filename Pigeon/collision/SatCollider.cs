using System;
using Microsoft.Xna.Framework;

namespace pigeon.collision {
    public static class SatCollider {
        public static bool CollideBoxPoint(Rectangle box, Point point) {
            return box.Right - 1 >= point.X
                && box.Bottom - 1 >= point.Y
                && box.Left <= point.X
                && box.Top <= point.Y;
        }

        public static bool CollideBoxes(Rectangle a, Rectangle b) {
            return a.Right - 1 >= b.Left
                && a.Bottom - 1 >= b.Top
                && a.Left <= b.Right - 1
                && a.Top <= b.Bottom - 1;
        }

        public static int GetMinTranslationX(Rectangle specA, Rectangle specB) {
            int left = (specB.Left - specA.Right);
            int right = (specB.Right - specA.Left);

            return left > 0 || right < 0 ? 0 : Math.Abs(left) < right ? left : right;
        }

        public static int GetMinTranslationY(Rectangle specA, Rectangle specB) {
            int top = (specB.Top - specA.Bottom);
            int bottom = (specB.Bottom - specA.Top);

            return top > 0 || bottom < 0 ? 0 : Math.Abs(top) < bottom ? top : bottom;
        }
    }
}
