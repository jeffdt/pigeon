using Microsoft.Xna.Framework;

namespace SampleGame.src.parameters {
    static class Display {
        public const int InitialScale = 2;
        public const int FrameRate = 144;
        public const int ScreenWidth = 47;
        public const int ScreenHeight = 25;

        public static readonly Point ScreenCenter = new Point(ScreenWidth / 2, ScreenHeight / 2);
        public static int ScreenCenterX => ScreenCenter.X;
        public static int ScreenCenterY => ScreenCenter.Y;

        public static readonly Point ScreenBoundary = new Point(ScreenWidth, ScreenHeight);
        public static readonly Rectangle Screen = new Rectangle(0, 0, ScreenBoundary.X, ScreenBoundary.Y);
    }
}
