using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using pigeon.input;
using pigeon.gameobject;
using pigeon.utilities;

namespace pigeon.component {
    public class SimpleController : Component {
        public int Speed = 100;

        private int lastX;
        private int lastY;

        protected override void Initialize() { }

        protected override void Update() {
            if (RawKeyboardInput.IsHeld(Keys.LeftShift, Keys.RightShift)) {
                Object.Stop();
                updateBumpControl();
            } else {
                updateSmoothControl();
            }
        }

        private void updateSmoothControl() {
            int newX;
            if (RawKeyboardInput.IsHeld(Keys.Left, Keys.A)) {
                newX = -1;
            } else if (RawKeyboardInput.IsHeld(Keys.Right, Keys.D)) {
                newX = 1;
            } else {
                newX = 0;
            }

            int newY;
            if (RawKeyboardInput.IsHeld(Keys.Up, Keys.W)) {
                newY = -1;
            } else if (RawKeyboardInput.IsHeld(Keys.Down, Keys.S)) {
                newY = 1;
            } else {
                newY = 0;
            }

            if (newX != lastX || newY != lastY) {
                Object.Velocity = new Vector2(newX * Speed, newY * Speed);
            }

            lastX = newX;
            lastY = newY;
        }

        private void updateBumpControl() {
            if (RawKeyboardInput.IsPressed(Keys.Left, Keys.A)) {
                Object.LocalPosition = Object.LocalPosition.PlusX(-1);
            } else if (RawKeyboardInput.IsPressed(Keys.Right, Keys.D)) {
                Object.LocalPosition = Object.LocalPosition.PlusX(1);
            }

            if (RawKeyboardInput.IsPressed(Keys.Up, Keys.W)) {
                Object.LocalPosition = Object.LocalPosition.PlusY(-1);
            } else if (RawKeyboardInput.IsPressed(Keys.Down, Keys.S)) {
                Object.LocalPosition = Object.LocalPosition.PlusY(1);
            }
        }
    }
}
