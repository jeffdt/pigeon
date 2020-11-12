using System;
using Microsoft.Xna.Framework;

namespace pigeon.utilities {
    public static class MathUtil {
        #region lerps
        public static float InverseLerp(this float value, float min, float max) {
            if (value <= min) {
                return 0;
            }

            return value >= max ? 1 : (value - min) / (max - min);
        }

        public static float CrossLerp(this float value, int fromMin, int fromMax, float toMin, float toMax) {
            var inverseLerp = InverseLerp(value, fromMin, fromMax);
            return MathHelper.Lerp(toMin, toMax, inverseLerp);
        }

        public static float CrossLerp(this int value, int fromMin, int fromMax, float toMin, float toMax) {
            return MathHelper.Lerp(toMin, toMax, InverseLerp(value, fromMin, fromMax));
        }
        #endregion

        public static float AngleBetween(float x1, float y1, float x2, float y2) {
            var a = MathHelper.ToDegrees((float) Math.Atan2(y2 - y1, x2 - x1));
            return a < 0 ? a + 360 : a;
        }

        public static float Distance(float x1, float y1, float x2, float y2) {
            return (float) Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
        }

        public static float Distance(Vector2 a, Vector2 b) {
            return (float) Math.Sqrt(((b.X - a.X) * (b.X - a.X)) + ((b.Y - a.Y) * (b.Y - a.Y)));
        }

        public static int Max(int a, int b) {
            return a >= b ? a : b;
        }

        public static float Max(float a, float b) {
            return a >= b ? a : b;
        }

        public static double Max(double a, double b) {
            return a >= b ? a : b;
        }

        public const double DEG = -180 / Math.PI;
        public const double RAD = Math.PI / -180;

        // inclusive
        public static bool IsInRange(float x, float min, float max) {
            return x >= min && x <= max;
        }

        public static bool IsInRange(Vector2 input, Vector2 min, Vector2 max) {
            return IsInRange(input.X, min.X, max.X) && IsInRange(input.Y, min.Y, max.Y);
        }

        public static bool IsContainedWithin(Vector2 input, Rectangle area) {
            return IsInRange(input.X, area.X, area.X + area.Width) && IsInRange(input.Y, area.Y, area.Y + area.Height);
        }
    }
}
