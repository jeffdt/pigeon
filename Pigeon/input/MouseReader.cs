using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace pigeon.input {
    public static class MouseReader {
        public static bool UpdateState = true;
        public static bool ReadWhenOutOfFocus = false;

        private static MouseState previousState;
        private static MouseState currentState;
        private static MouseState dummyState;

        public static void Initialize() {
            dummyState = new MouseState();
            currentState = Mouse.GetState();
        }

        public static void Update() {
            if (UpdateState) {
                previousState = currentState;
                currentState = Pigeon.IsInFocus || ReadWhenOutOfFocus ? Mouse.GetState() : dummyState;
            }
        }

        public static int X {
            get {
                return currentState.X / Pigeon.Renderer.DrawScale;
            }
        }

        public static int Y {
            get {
                return currentState.Y / Pigeon.Renderer.DrawScale;
            }
        }

        public static Point Position {
            get {
                return new Point(X, Y);
            }
        }

        public static bool IsLeftHeld() {
            return currentState.LeftButton == ButtonState.Pressed;
        }

        public static bool IsLeftJustPressed() {
            return currentState.LeftButton == ButtonState.Pressed && previousState.LeftButton == ButtonState.Released;
        }

        public static bool IsLeftJustReleased() {
            return currentState.LeftButton == ButtonState.Released && previousState.LeftButton == ButtonState.Pressed;
        }

        public static bool IsRightHeld() {
            return currentState.RightButton == ButtonState.Pressed;
        }

        public static bool IsRightJustPressed() {
            return currentState.RightButton == ButtonState.Pressed && previousState.RightButton == ButtonState.Released;
        }

        public static bool IsRightJustReleased() {
            return currentState.RightButton == ButtonState.Released && previousState.RightButton == ButtonState.Pressed;
        }

        public static bool IsWheelScrolledUp() {
            return currentState.ScrollWheelValue > previousState.ScrollWheelValue;
        }

        public static bool IsWheelScrolledDown() {
            return currentState.ScrollWheelValue < previousState.ScrollWheelValue;
        }
    }
}
