using Microsoft.Xna.Framework;

namespace pigeon.time {
    public static class Time {
        public static float Milliseconds;
        public static float Seconds;
        public static float MsScaled;
        public static float SecScaled;
        public static double TotalSec;

        public static double SingleFrame { get { return Pigeon.Instance.TargetElapsedTime.Seconds; } }

        public static void Set(GameTime gameTime) {
            Milliseconds = (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            Seconds = (float) gameTime.ElapsedGameTime.TotalSeconds;

            MsScaled = Milliseconds * GameSpeed.Multiplier;
            SecScaled = Seconds * GameSpeed.Multiplier;

            TotalSec = gameTime.TotalGameTime.TotalSeconds;
        }
    }
}
