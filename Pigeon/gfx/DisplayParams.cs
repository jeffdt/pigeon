namespace pigeon.gfx {
    public class DisplayParams {
        public DisplayParams(int screenWidth, int screenHeight, int initialScale, int frameRate) {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            InitialScale = initialScale;
            FrameRate = frameRate;
        }

        public int ScreenWidth { get; }
        public int ScreenHeight { get; }
        public int InitialScale { get; }
        public int FrameRate { get; }
    }
}
