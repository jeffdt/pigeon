using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace pigeon.collision {
    public static class BresenhamLine {
        // Swap the values of A and B
        private static void swap<T>(ref T a, ref T b) {
            T c = a;
            a = b;
            b = c;
        }

        // Returns the list of points from p0 to p1 
        public static List<Point> FindLine(Point p0, Point p1) {
            return FindLine(p0.X, p0.Y, p1.X, p1.Y);
        }

        // Returns the list of points from (x0, y0) to (x1, y1)
        public static List<Point> FindLine(int x0, int y0, int x1, int y1) {
            // Optimization: it would be preferable to calculate in
            // advance the size of "result" and to use a fixed-size array
            // instead of a list.
            List<Point> result = new List<Point>();

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep) {
                swap(ref x0, ref y0);
                swap(ref x1, ref y1);
            }
            if (x0 > x1) {
                swap(ref x0, ref x1);
                swap(ref y0, ref y1);
            }

            int deltax = x1 - x0;
            int deltay = Math.Abs(y1 - y0);
            int error = 0;
            int ystep;
            int y = y0;
            if (y0 < y1)
                ystep = 1;
            else
                ystep = -1;
            for (int x = x0; x <= x1; x++) {
                result.Add(steep ? new Point(y, x) : new Point(x, y));
                error += deltay;
                if (2 * error >= deltax) {
                    y += ystep;
                    error -= deltax;
                }
            }

            return result;
        }
    }
}
