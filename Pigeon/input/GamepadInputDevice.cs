using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace pigeon.input {
    public class GamepadInputDevice : InputDevice {
        // CONSTANTS
        private const float JOYSTICK_DIGITAL_LENGTH_THRESH = .5f;
        private const float JOYSTICK_COMPONENT_THRESH = .25f;

        // PARAMETERS
        private readonly PlayerIndex playerIndex;
        private readonly Buttons[] map;

        // STATE
        private readonly GamePadState emptyState;
        private GamePadState previousState;
        private GamePadState currentState;

        private bool currentLeftJoystickActive;
        private bool previousLeftJoystickActive;

        public GamepadInputDevice(int controllerNumber, Buttons[] buttonMap) {
            switch (controllerNumber) {
                case 0:
                    playerIndex = PlayerIndex.One;
                    break;
                case 1:
                    playerIndex = PlayerIndex.Two;
                    break;
                case 2:
                    playerIndex = PlayerIndex.Three;
                    break;
                case 3:
                    playerIndex = PlayerIndex.Four;
                    break;
            }

            map = buttonMap;

            currentState = GamePad.GetState(playerIndex);
            emptyState = new GamePadState();
        }

        public override void Update() {
            previousState = currentState;
            currentState = Pigeon.IsInFocus ? GamePad.GetState(playerIndex) : emptyState;

            if (currentState.IsConnected) {
                previousLeftJoystickActive = currentLeftJoystickActive;
                currentLeftJoystickActive = currentState.ThumbSticks.Left.Length() > JOYSTICK_DIGITAL_LENGTH_THRESH;
            }
        }

        public override bool IsButtonPressed(int buttonNumber) {
            if (!currentState.IsConnected) {
                return false;
            }

            Buttons button = map[buttonNumber];
            bool result = isButtonPressedRaw(button);

            switch (button) {
                case Buttons.DPadUp:
                    return result || isJoystickPressedUp();
                case Buttons.DPadRight:
                    return result || isJoystickPressedRight();
                case Buttons.DPadDown:
                    return result || isJoystickPressedDown();
                case Buttons.DPadLeft:
                    return result || isJoystickPressedLeft();
            }

            return result;
        }

        public override bool IsButtonHeld(int buttonNumber) {
            if (!currentState.IsConnected) {
                return false;
            }

            Buttons button = map[buttonNumber];
            bool result = isButtonHeldRaw(map[buttonNumber]);

            switch (button) {
                case Buttons.DPadUp:
                    return result || IsJoystickHeldUp();
                case Buttons.DPadRight:
                    return result || IsJoystickHeldRight();
                case Buttons.DPadDown:
                    return result || IsJoystickHeldDown();
                case Buttons.DPadLeft:
                    return result || IsJoystickHeldLeft();
            }

            return result;
        }

        public override bool IsAnyPressed() {
            return isButtonPressedRaw(Buttons.A) || isButtonPressedRaw(Buttons.B)
                || isButtonPressedRaw(Buttons.X) || isButtonPressedRaw(Buttons.Y)
                || isButtonPressedRaw(Buttons.LeftShoulder) || isButtonPressedRaw(Buttons.RightShoulder)
                || isButtonPressedRaw(Buttons.LeftTrigger) || isButtonPressedRaw(Buttons.RightTrigger)
                || isButtonPressedRaw(Buttons.DPadLeft) || isButtonPressedRaw(Buttons.DPadRight)
                || isButtonPressedRaw(Buttons.DPadUp) || isButtonPressedRaw(Buttons.DPadDown)
                || isButtonPressedRaw(Buttons.Back) || isButtonPressedRaw(Buttons.Start)
                || isJoystickPressedUp() || isJoystickPressedRight() || isJoystickPressedDown() || isJoystickPressedLeft();
        }

        public override bool IsAnyHeld() {
            return isButtonHeldRaw(Buttons.A) || isButtonHeldRaw(Buttons.B)
                || isButtonHeldRaw(Buttons.X) || isButtonHeldRaw(Buttons.Y)
                || isButtonHeldRaw(Buttons.LeftShoulder) || isButtonHeldRaw(Buttons.RightShoulder)
                || isButtonHeldRaw(Buttons.LeftTrigger) || isButtonHeldRaw(Buttons.RightTrigger)
                || isButtonHeldRaw(Buttons.DPadLeft) || isButtonHeldRaw(Buttons.DPadRight)
                || isButtonHeldRaw(Buttons.DPadUp) || isButtonHeldRaw(Buttons.DPadDown)
                || isButtonHeldRaw(Buttons.Back) || isButtonHeldRaw(Buttons.Start)
                || isButtonHeldRaw(Buttons.LeftStick) || isButtonHeldRaw(Buttons.RightStick)
                || IsJoystickHeldUp() || IsJoystickHeldRight() || IsJoystickHeldDown() || IsJoystickHeldLeft();
        }

        #region raw input checks
        private bool isButtonPressedRaw(Buttons button) {
            return currentState.IsButtonDown(button) && previousState.IsButtonUp(button);
        }

        private bool isButtonHeldRaw(Buttons button) {
            return currentState.IsButtonDown(button);
        }

        private bool isJoystickPressedUp() {
            return !previousLeftJoystickActive && IsJoystickHeldUp();
        }

        private bool isJoystickPressedRight() {
            return !previousLeftJoystickActive && IsJoystickHeldRight();
        }

        private bool isJoystickPressedDown() {
            return !previousLeftJoystickActive && IsJoystickHeldDown();
        }

        private bool isJoystickPressedLeft() {
            return !previousLeftJoystickActive && IsJoystickHeldLeft();
        }

        public bool IsJoystickHeldUp() {
            return currentLeftJoystickActive && currentState.ThumbSticks.Left.Y > JOYSTICK_COMPONENT_THRESH;
        }

        public bool IsJoystickHeldDown() {
            return currentLeftJoystickActive && currentState.ThumbSticks.Left.Y < -JOYSTICK_COMPONENT_THRESH;
        }

        public bool IsJoystickHeldRight() {
            return currentLeftJoystickActive && currentState.ThumbSticks.Left.X > JOYSTICK_COMPONENT_THRESH;
        }

        public bool IsJoystickHeldLeft() {
            return currentLeftJoystickActive && currentState.ThumbSticks.Left.X < -JOYSTICK_COMPONENT_THRESH;
        }
        #endregion
    }
}
