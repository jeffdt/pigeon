using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PigeonEngine.utilities {
    public static class LineCalculator {
        private static Point[] findLine(Point a, Point b) {
            List<Point> coords = new List<Point>();

            int dx1 = 0;
            int dy1 = 0;
            int dx2 = 0;
            int dy2 = 0;

            int w = b.X - a.X;
            int h = b.Y - a.Y;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);

            if (w < 0)
                dx1 = -1;
            else if (w > 0)
                dx1 = 1;

            if (h < 0)
                dy1 = -1;
            else if (h > 0)
                dy1 = 1;

            if (w < 0)
                dx2 = -1;
            else if (w > 0)
                dx2 = 1;

            if (!(longest > shortest)) {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0)
                    dy2 = -1;
                else if (h > 0)
                    dy2 = 1;
                dx2 = 0;
            }

            int numerator = longest >> 1;

            int x = a.X;
            int y = a.Y;

            for (int i = 0; i <= longest; i++) {
                coords.Add(new Point(x, y));
                numerator += shortest;
                if (!(numerator < longest)) {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                } else {
                    x += dx2;
                    y += dy2;
                }
            }

            return coords.ToArray();
        }
    }
}
