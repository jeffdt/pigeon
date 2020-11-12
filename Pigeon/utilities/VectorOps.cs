using System;
using Microsoft.Xna.Framework;

namespace pigeon.utilities {
    public static class VectorOps {
        public static Vector2 DiscardMax(this Vector2 vect) {
            return (vect.X * vect.X) > (vect.Y * vect.Y) ? new Vector2(0, vect.Y) : new Vector2(vect.X, 0);
        }

        public static string ToFormattedString(this Vector2 vect) {
            return string.Format("({0},{1})", vect.X, vect.Y);
        }

        public static Vector2 ClampLength(this Vector2 vect, float min, float max) {
            var length = vect.Length();

            if (length < min) {
                return vect.Scale(min);
            }

            return length > max ? vect.Scale(max) : vect;
        }

        public static Vector2 Clamp(this Vector2 vect, Vector2 clampMin, Vector2 clampMax) {
            return new Vector2(
                MathHelper.Clamp(vect.X, clampMin.X, clampMax.X),
                MathHelper.Clamp(vect.Y, clampMin.Y, clampMax.Y)
            );
        }

        public static Vector2 SetX(this Vector2 vector, float x) {
            return new Vector2(x, vector.Y);
        }

        public static Vector2 SetY(this Vector2 vector, float y) {
            return new Vector2(vector.X, y);
        }

        #region position adjustments
        public static Vector2 ClampToArea(this Vector2 vector, Rectangle area) {
            vector.X = MathHelper.Clamp(vector.X, area.X, area.X + area.Width);
            vector.Y = MathHelper.Clamp(vector.Y, area.Y, area.Y + area.Height);
            return vector;
        }
        #endregion

        #region scale
        public static Vector2 SafeNormalize(this Vector2 vector) {
            if (vector == Vector2.Zero) {
                return Vector2.Zero;
            }

            vector.Normalize();

            return vector;
        }

        public static Vector2 Scale(this Vector2 vector, float length) {
            return SafeNormalize(vector) * length;
        }
        #endregion

        #region direction alignment operations
        public static bool IsLeft(this Vector2 vector) {
            return vector.X < 0 && vector.Y == 0;
        }

        public static bool IsRight(this Vector2 vector) {
            return vector.X > 0 && vector.Y == 0;
        }

        public static bool IsUp(this Vector2 vector) {
            return vector.X == 0 && vector.Y < 0;
        }

        public static bool IsDown(this Vector2 vector) {
            return vector.X == 0 && vector.Y > 0;
        }

        public static bool IsUpLeft(this Vector2 vector) {
            return vector.X < 0 && vector.Y < 0;
        }

        public static bool IsUpRight(this Vector2 vector) {
            return vector.X > 0 && vector.Y < 0;
        }

        public static bool IsDownLeft(this Vector2 vector) {
            return vector.X < 0 && vector.Y > 0;
        }

        public static bool IsDownRight(this Vector2 vector) {
            return vector.X > 0 && vector.Y > 0;
        }

        public static Vector2 AlignToEightWay(this Vector2 vector) {
            var degrees = DegreesFromYAxis(vector);

            if (degrees <= -157.5f || degrees >= 157.5) { // d
                return new Vector2(0, 1);
            }

            if (degrees > 112.5) { // dr
                return new Vector2(1, 1);
            }

            if (degrees > 67.5) { // r
                return new Vector2(1, 0);
            }

            if (degrees > 22.5) { // ur
                return new Vector2(1, -1);
            }

            if (degrees >= 0 || degrees > -22.5) { // u
                return new Vector2(0, -1);
            }

            if (degrees > -67.5) { // ul
                return new Vector2(-1, -1);
            }

            if (degrees > -112.5) { // l
                return new Vector2(-1, 0);
            }

            return new Vector2(-1, 1); // dl
        }
        #endregion

        #region angle operations
        public static float Cos(Vector2 vector1, Vector2 vector2) {
            return Vector2.Dot(SafeNormalize(vector1), SafeNormalize(vector2));
        }

        public static float RadiansFromYAxis(Vector2 vector) {  // "0" is up
            return (float) Math.Atan2(vector.X, -vector.Y);
        }

        public static float DegreesFromYAxis(Vector2 vector) {	// "0" is up
            return MathHelper.ToDegrees(RadiansFromYAxis(vector));
        }

        public static float RadiansFromXAxis(Vector2 vector) {  // "0" is right
            return (float) Math.Atan2(-vector.Y, vector.X);
        }

        public static float DegreesFromXAxis(Vector2 vector) {  // "0" is right
            return MathHelper.ToDegrees(RadiansFromXAxis(vector));
        }

        public static Vector2 FromDegrees(float angle, float length = 1) {
            return FromRadians(MathHelper.ToRadians(angle), length);
        }

        public static Vector2 FromRadians(float angleInRadians, float length = 1) {
            Vector2 angledVector = new Vector2((float) Math.Cos(angleInRadians), (float) -Math.Sin(angleInRadians));
            return length == 1 ? angledVector : Scale(angledVector, length);
        }

        public static Vector2 AddAngleDegrees(Vector2 vector, float angle) {
            return FromDegrees(DegreesFromXAxis(vector) + angle, vector.Length());
        }
        #endregion
    }
}
