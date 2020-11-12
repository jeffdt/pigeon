using System;
using Microsoft.Xna.Framework;

namespace pigeon.utilities {
    public static class PointExtensions {
        public static string ToFormattedString(this Point pt) {
            return string.Format("({0},{1})", pt.X, pt.Y);
        }

        public static Point DiscardMax(this Point point) {
            return (point.X * point.X) > (point.Y * point.Y) ? new Point(0, point.Y) : new Point(point.X, 0);
        }

        public static Point WithX(this Point point, int x) {
            return new Point(x, point.Y);
        }

        public static Point WithY(this Point point, int y) {
            return new Point(point.X, y);
        }

        public static Point WithoutX(this Point point) {
            return new Point(0, point.Y);
        }

        public static Point WithoutY(this Point point) {
            return new Point(point.X, 0);
        }

        public static Point PlusX(this Point point, int x) {
            return new Point(point.X + x, point.Y);
        }

        public static Point PlusY(this Point point, int y) {
            return new Point(point.X, point.Y + y);
        }

        public static Point Plus(this Point point, int val) {
            return new Point(point.X + val, point.Y + val);
        }

        public static Point MinusX(this Point point, int x) {
            return new Point(point.X - x, point.Y);
        }

        public static Point MinusY(this Point point, int y) {
            return new Point(point.X, point.Y - y);
        }

        public static Point Div(this Point point, int divisor) {
            return new Point(point.X / divisor, point.Y / divisor);
        }

        public static Point Mult(this Point point, int multiplier) {
            return new Point(point.X * multiplier, point.Y * multiplier);
        }

        public static int LengthSquared(this Point point) {
            return (point.X * point.X) + (point.Y * point.Y);
        }

        public static double Length(this Point point) {
            return Math.Sqrt((point.X * point.X) + (point.Y * point.Y));
        }

        public static Point Clamp(this Point point, Point clampMin, Point clampMax) {
            return new Point(
                MathHelper.Clamp(point.X, clampMin.X, clampMax.X),
                MathHelper.Clamp(point.Y, clampMin.Y, clampMax.Y)
            );
        }

        public static bool IsTargetReached(this Point targetPos, Point currentPos, Vector2 travelDir) {
            // TODO: can this be simplified? e.g. check if sign matches for vector to target and travel dir

            var toTarget = targetPos - currentPos;

            return ((toTarget.X > 0 || travelDir.X > 0) && (toTarget.X < 0 || travelDir.X < 0))
                || ((toTarget.Y > 0 || travelDir.Y > 0) && (toTarget.Y < 0 || travelDir.Y < 0));

            //			if (travelDir.X < 0 && travelDir.Y == 0) { // left
            //				if (currentPos.X < targetPos.X) {
            //					return true;
            //				}
            //			} else if (travelDir.X > 0 && travelDir.Y == 0) { // right
            //				if (currentPos.X > targetPos.X) {
            //					return true;
            //				}
            //			} else if (travelDir.X == 0 && travelDir.Y < 0) { // up
            //				if (currentPos.Y < targetPos.Y) {
            //					return true;
            //				}
            //			} else if (travelDir.X == 0 && travelDir.Y > 0) { // down
            //				if (currentPos.Y > targetPos.Y) {
            //					return true;
            //				} 
            //			} else if (travelDir.X < 0 && travelDir.Y < 0) { // upleft
            //				if (currentPos.X < targetPos.X && currentPos.Y < targetPos.Y) {
            //					return true;
            //				} 
            //			} else if (travelDir.X > 0 && travelDir.Y < 0) { // upright
            //				if (currentPos.X > targetPos.X && currentPos.Y < targetPos.Y) {
            //					return true;
            //				}  
            //			} else if (travelDir.X < 0 && travelDir.Y > 0) { // downleft
            //				if (currentPos.X < targetPos.X && currentPos.Y > targetPos.Y) {
            //					return true;
            //				} 
            //			} else if (travelDir.X > 0 && travelDir.Y > 0) { // downright
            //				if (currentPos.X > targetPos.X && currentPos.Y > targetPos.Y) {
            //					return true;
            //				} 
            //			} else {
            //				// TODO: remove this after debugging
            //				throw new ArgumentException("no travel dir");
            //			}
        }
    }
}
