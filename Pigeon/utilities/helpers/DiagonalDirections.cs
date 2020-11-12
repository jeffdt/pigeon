using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigeonEngine.utilities.helpers {
    public enum Diagonals {
        UpLeft = 0, UpRight = 1, DownRight = 2, DownLeft = 3
    }

    public static class DiagonalsExtensions {
        private static readonly Point[] points = new Point[] { new Point(-1, -1), new Point(1, -1), new Point(1, 1), new Point(-1, 1) };
        private static readonly Vector2[] vectors = new Vector2[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) };

        public static int ToInt(this Diagonals diagonal) {
            return (int) diagonal;
        }

        public static int ToOppositeInt(this Diagonals diagonal) {
            return (int) diagonal % 4;
        }

        public static Point ToPoint(this Diagonals diagonal) {
            return points[(int) diagonal];
        }

        public static Point ToOppositePoint(this Diagonals diagonal) {
            return points[(int) diagonal % 4];
        }

        public static Vector2 ToVector2(this Diagonals diagonal) {
            return vectors[(int) diagonal];
        }
    }
}