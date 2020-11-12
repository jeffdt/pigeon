using System;

namespace SampleGame {
    public static class Program {
        [STAThread]
        static void Main() {
            using var game = new SampleGame();
            game.Run();
        }
    }
}
