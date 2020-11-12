using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using pigeon.utilities.helpers;

namespace pigeon.input {
    public class Legacy_GamepadAdapter<T> {
        private readonly Dictionary<T, Buttons> buttomMap;

        public Legacy_GamepadAdapter(Dictionary<T, Buttons> initialMapping) {
            buttomMap = new Dictionary<T, Buttons>(initialMapping);
        }

        public Buttons GetMappedKey(T input) {
            return buttomMap[input];
        }

        public void RemapInputs(Dictionary<T, Buttons> newInputMapping) {
            foreach (var key in newInputMapping.Keys) {
                buttomMap[key] = newInputMapping[key];
            }
        }

        public bool IsPressed(T input) {
            return RawGamepadInput.IsPressed(buttomMap[input]);
        }

        public bool IsHeld(T input) {
            return RawGamepadInput.IsHeld(buttomMap[input]);
        }

        public bool IsLJSPressed(AxisDirections direction) {
            switch (direction) {
                case AxisDirections.Up:
                    return RawGamepadInput.IsLeftJoystickPressedUp();
                case AxisDirections.Down:
                    return RawGamepadInput.IsLeftJoystickPressedDown();
                case AxisDirections.Left:
                    return RawGamepadInput.IsLeftJoystickPressedLeft();
                default:
                    return RawGamepadInput.IsLeftJoystickPressedRight();
            }
        }

        public bool IsLJSHeld(AxisDirections direction) {
            switch (direction) {
                case AxisDirections.Up:
                    return RawGamepadInput.IsLeftJoystickHeldUp();
                case AxisDirections.Down:
                    return RawGamepadInput.IsLeftJoystickHeldDown();
                case AxisDirections.Left:
                    return RawGamepadInput.IsLeftJoystickHeldLeft();
                default:
                    return RawGamepadInput.IsLeftJoystickHeldRight();
            }
        }

        public bool IsDpadPressed(AxisDirections direction) {
            return RawGamepadInput.IsDpadPressed(direction);
        }

        public bool IsDpadHeld(AxisDirections direction) {
            return RawGamepadInput.IsDpadHeld(direction);
        }
    }
}