using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using pigeon.utilities.helpers;

namespace pigeon.input {
    public static class RawGamepadInput {
        private const float JOYSTICK_DIGITAL_LENGTH_THRESH = .5f;
        private const float JOYSTICK_COMPONENT_THRESH = .25f;

        private static GamePadState emptyState;
        private static GamePadState previousState;
        private static GamePadState currentState;

        private static bool currentLeftJoystickActive;
        private static bool previousLeftJoystickActive;
        private const PlayerIndex controllerIndex = PlayerIndex.One;

        public static void Initialize() {
            currentState = getPlayerOneState();
            emptyState = new GamePadState();
        }

        public static void Update() {
            previousState = currentState;
            currentState = Pigeon.IsInFocus ? getPlayerOneState() : emptyState;

            previousLeftJoystickActive = currentLeftJoystickActive;
            currentLeftJoystickActive = currentState.ThumbSticks.Left.Length() > JOYSTICK_DIGITAL_LENGTH_THRESH;
        }

        private static GamePadState getPlayerOneState() {
            return GamePad.GetState(controllerIndex);
        }

        public static bool IsPressed(Buttons button) {
            return currentState.IsButtonDown(button) && previousState.IsButtonUp(button) && Pigeon.IsInFocus;
        }

        public static bool IsHeld(Buttons button) {
            return currentState.IsButtonDown(button) && Pigeon.IsInFocus;
        }

        public static bool IsLeftJoystickPressedUp() {
            return !previousLeftJoystickActive && IsLeftJoystickHeldUp() && Pigeon.IsInFocus;
        }

        public static bool IsLeftJoystickPressedDown() {
            return !previousLeftJoystickActive && IsLeftJoystickHeldDown() && Pigeon.IsInFocus;
        }

        public static bool IsLeftJoystickPressedLeft() {
            return !previousLeftJoystickActive && IsLeftJoystickHeldLeft() && Pigeon.IsInFocus;
        }

        public static bool IsLeftJoystickPressedRight() {
            return !previousLeftJoystickActive && IsLeftJoystickHeldRight() && Pigeon.IsInFocus;
        }

        public static bool IsLeftJoystickPressed(AxisDirections dir) {
            switch (dir) {
                case AxisDirections.Up:
                    return  IsLeftJoystickPressedUp();
                case AxisDirections.Down:
                    return IsLeftJoystickPressedDown();
                case AxisDirections.Left:
                    return IsLeftJoystickPressedLeft();
                default:
                    return IsLeftJoystickPressedRight();
            }
        }

        public static bool IsLeftJoystickHeldUp() {
            return currentLeftJoystickActive && currentState.ThumbSticks.Left.Y > JOYSTICK_COMPONENT_THRESH && Pigeon.IsInFocus;
        }

        public static bool IsLeftJoystickHeldDown() {
            return currentLeftJoystickActive && currentState.ThumbSticks.Left.Y < -JOYSTICK_COMPONENT_THRESH && Pigeon.IsInFocus;
        }

        public static bool IsLeftJoystickHeldLeft() {
            return currentLeftJoystickActive && currentState.ThumbSticks.Left.X < -JOYSTICK_COMPONENT_THRESH && Pigeon.IsInFocus;
        }

        public static bool IsLeftJoystickHeldRight() {
            return currentLeftJoystickActive && currentState.ThumbSticks.Left.X > JOYSTICK_COMPONENT_THRESH && Pigeon.IsInFocus;
        }

        public static bool IsDpadPressed(AxisDirections direction) {
            bool pressed;

            switch (direction) {
                case AxisDirections.Up:
                    pressed = currentState.IsButtonDown(Buttons.DPadUp) && previousState.IsButtonUp(Buttons.DPadUp);
                    break;
                case AxisDirections.Down:
                    pressed = currentState.IsButtonDown(Buttons.DPadDown) && previousState.IsButtonUp(Buttons.DPadDown);
                    break;
                case AxisDirections.Left:
                    pressed = currentState.IsButtonDown(Buttons.DPadLeft) && previousState.IsButtonUp(Buttons.DPadLeft);
                    break;
                default:
                    pressed = currentState.IsButtonDown(Buttons.DPadRight) && previousState.IsButtonUp(Buttons.DPadRight);
                    break;
            }

            return pressed && Pigeon.IsInFocus;
        }

        public static bool IsDpadHeld(AxisDirections direction) {
            bool pressed;

            switch (direction) {
                case AxisDirections.Up:
                    pressed = currentState.IsButtonDown(Buttons.DPadUp);
                    break;
                case AxisDirections.Down:
                    pressed = currentState.IsButtonDown(Buttons.DPadDown);
                    break;
                case AxisDirections.Left:
                    pressed = currentState.IsButtonDown(Buttons.DPadLeft);
                    break;
                default:
                    pressed = currentState.IsButtonDown(Buttons.DPadRight);
                    break;
            }

            return pressed && Pigeon.IsInFocus;
        }

        public static bool IsAnyButtonJustPressed() {
            return IsPressed(Buttons.A) || IsPressed(Buttons.B)
                || IsPressed(Buttons.X) || IsPressed(Buttons.Y)
                || IsPressed(Buttons.LeftShoulder) || IsPressed(Buttons.RightShoulder)
                || IsPressed(Buttons.LeftTrigger) || IsPressed(Buttons.RightTrigger)
                || IsPressed(Buttons.DPadLeft) || IsPressed(Buttons.DPadRight)
                || IsPressed(Buttons.DPadUp) || IsPressed(Buttons.DPadDown)
                || IsPressed(Buttons.Back) || IsPressed(Buttons.Start)
                || IsPressed(Buttons.LeftStick) || IsPressed(Buttons.RightStick);
        }

        public static bool IsAnyButtonHeld() {
            return IsHeld(Buttons.A) || IsHeld(Buttons.B)
                || IsHeld(Buttons.X) || IsHeld(Buttons.Y)
                || IsHeld(Buttons.LeftShoulder) || IsHeld(Buttons.RightShoulder)
                || IsHeld(Buttons.LeftTrigger) || IsHeld(Buttons.RightTrigger)
                || IsHeld(Buttons.DPadLeft) || IsHeld(Buttons.DPadRight)
                || IsHeld(Buttons.DPadUp) || IsHeld(Buttons.DPadDown)
                || IsHeld(Buttons.Back) || IsHeld(Buttons.Start)
                || IsHeld(Buttons.LeftStick) || IsHeld(Buttons.RightStick);
        }
    }
}
