using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using pigeon.utilities.helpers;

namespace pigeon.input {
    public class Legacy_KeyboardAdapter<T> {
        private readonly Dictionary<T, Keys> keyMap;

        public Legacy_KeyboardAdapter(Dictionary<T, Keys> initialMapping) {
            keyMap = new Dictionary<T, Keys>(initialMapping);
        }

        public Keys GetMappedKey(T input) {
            return keyMap[input];
        }

        public void RemapInputs(Dictionary<T, Keys> newInputMapping) {
            foreach (var key in newInputMapping.Keys) {
                keyMap[key] = newInputMapping[key];
            }
        }

        public bool IsPressed(AxisDirections dir) {
            switch (dir) {
                case AxisDirections.Up:
                    return RawKeyboardInput.IsPressed(Keys.Up);
                case AxisDirections.Down:
                    return RawKeyboardInput.IsPressed(Keys.Down);
                case AxisDirections.Left:
                    return RawKeyboardInput.IsPressed(Keys.Left);
                default:
                    return RawKeyboardInput.IsPressed(Keys.Right);
            }
        }

        public bool IsPressed(T input) {
            return RawKeyboardInput.IsPressed(keyMap[input]);
        }

        public bool IsHeld(AxisDirections dir) {
            switch (dir) {
                case AxisDirections.Up:
                    return RawKeyboardInput.IsHeld(Keys.Up);
                case AxisDirections.Down:
                    return RawKeyboardInput.IsHeld(Keys.Down);
                case AxisDirections.Left:
                    return RawKeyboardInput.IsHeld(Keys.Left);
                default:
                    return RawKeyboardInput.IsHeld(Keys.Right);
            }
        }

        public bool IsHeld(T input) {
            return RawKeyboardInput.IsHeld(keyMap[input]);
        }
    }
}