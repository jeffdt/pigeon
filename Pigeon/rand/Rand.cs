using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using pigeon.utilities;
using pigeon.utilities.helpers;

namespace pigeon.rand {
    public static class Rand {
        private static readonly Random rand = new Random();

        public static Vector2 FlatUnsignedVector(int maxX, int maxY) {
            return new Vector2(rand.Next(maxX), rand.Next(maxY));
        }

        public static Vector2 FlatSignedVector(int maxX, int maxY) {
            return new Vector2(rand.Next(-maxX, maxX), rand.Next(-maxY, maxY));
        }

        public static Vector2 SignedVector(float maxX, float maxY) {
            float randX = (float) (rand.NextDouble() * maxX * SignFloat());
            float randY = (float) (rand.NextDouble() * maxY * SignFloat());

            return new Vector2(randX, randY);
        }

        public static Vector2 AngledVector(float length = 1) {
            return VectorOps.FromRadians(AngleInRadians(), length);
        }

        public static int Int(int max) {
            return rand.Next(0, max);
        }

        public static int IntInclusive(int min, int max) {
            return Int(min, max + 1);
        }

        public static int Int(int min, int max) {
            return rand.Next(min, max);
        }

        public static int Random(this int integer) {
            return Int(integer);
        }

        public static double Double() {
            return rand.NextDouble();
        }

        public static float Float(float min, float max) {
            return (float) (min + (Double() * (max - min)));
        }

        public static int SignInt() {
            return rand.Next(2) == 1 ? 1 : -1;
        }

        public static float SignFloat() {
            return rand.Next(2) == 1 ? 1f : -1f;
        }

        public static T From<T>(params T[] elements) {
            return elements[Int(elements.Length)];
        }

        public static T RandomElement<T>(this List<T> list) {
            return list[Int(list.Count)];
        }

        public static T RandomElement<T>(this T[] array) {
            return array[Int(array.Length)];
        }

        public static bool Bool() {
            return rand.Next(2) == 1;
        }

        public static float AngleInRadians() {
            return (float) (Double() * MathHelper.TwoPi);
        }

        public static Vector2 BorderPosition(Rectangle area, int extraOffset = 0) {
            Vector2 position = Vector2.Zero;

            switch (Int(4)) {
                case 0: // top
                    position.X = area.X + Int(area.Width);
                    position.Y = area.Y - extraOffset;
                    break;
                case 1: // bottom
                    position.X = area.X + Int(area.Width);
                    position.Y = area.Y + area.Height + extraOffset;
                    break;
                case 2: // left
                    position.X = area.X - extraOffset;
                    position.Y = area.Y + Int(area.Height);
                    break;
                default:    // right
                    position.X = area.X + area.Width + extraOffset;
                    position.Y = area.Y + Int(area.Height);
                    break;
            }
            return position;
        }

        public static Point RandPointInside(this Rectangle rect) {
            return new Point(Int(rect.Left, rect.Right), Int(rect.Top, rect.Bottom));
        }

        public static Point RandPositionWithinArea(this Rectangle area) {
            return RandPositionWithinArea(area, Point.Zero);
        }

        public static Point RandPositionWithinArea(this Rectangle area, Point minDistanceFromBorder) {
            var left = area.X + minDistanceFromBorder.X;
            var top = area.Y + minDistanceFromBorder.Y;
            var right = area.X + area.Width - minDistanceFromBorder.X;
            var bottom = area.Y + area.Height - minDistanceFromBorder.Y;

            var x = Int(left, right);
            var y = Int(top, bottom);

            return new Point(x, y);
        }

        public static T EnumValue<T>() {
            Array values = Enum.GetValues(typeof(T));
            return (T) values.GetValue(rand.Next(values.Length));
        }

        public static void Shuffle<T>(this IList<T> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = Int(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static Vector2 Direction8Way() {
            int dir = Int(8);

            switch (dir) {
                case 0:
                    return EightWayDirections.UpLeft;
                case 1:
                    return EightWayDirections.Up;
                case 2:
                    return EightWayDirections.UpRight;
                case 3:
                    return EightWayDirections.Left;
                case 4:
                    return EightWayDirections.Right;
                case 5:
                    return EightWayDirections.DownLeft;
                case 6:
                    return EightWayDirections.Down;
                default:
                    return EightWayDirections.DownRight;
            }
        }
    }
}
