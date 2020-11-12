using Microsoft.Xna.Framework;
using pigeon.utilities.extensions;

namespace pigeon.console {
    public class ConsoleOptions {
        public int PanelHeight = Pigeon.Renderer.BaseResolutionY;
        public Color PanelColor = Color.Navy.WithAlpha(150);
        public Color BufferColor = Color.SkyBlue;
        public Color CursorColor = Color.White;
        public Color HistoryColor = Color.SkyBlue;
        public Color InfoColor = Color.MintCream;
        public Color ErrorColor = Color.Red;

        public int LogHistoryLimit = 64;
        public int CommandHistory = 16;

        public int TextInset = 5;
    }
}
