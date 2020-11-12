using Microsoft.Xna.Framework;
using System;

namespace pigeon.utilities.helpers {
    public enum AxisDirections {
        Up = 0, Right = 1, Down = 2, Left = 3
    }

    public static class AxisDirectionsExtensions {
        private static readonly Point[] points = new Point[] { new Point(0, -1), new Point(1, 0), new Point(0, 1), new Point(-1, 0) };

        public static int ToInt(this AxisDirections axisDir) {
            return (int) axisDir;
        }

        public static Point ToPoint(this AxisDirections axisDir) {
            return points[(int) axisDir];
        }

        public static AxisDirections Opposite(this AxisDirections sd) {
            switch (sd) {
                case AxisDirections.Up:
                    return AxisDirections.Down;
                case AxisDirections.Down:
                    return AxisDirections.Up;
                case AxisDirections.Left:
                    return AxisDirections.Right;
                case AxisDirections.Right:
                    return AxisDirections.Left;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sd));
            }
        }
    }
}
