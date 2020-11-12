using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace pigeon.input {
    public static class RawKeyboardInput {
        private static KeyboardState previousState;
        private static KeyboardState currentState;

        private static KeyboardState dummyState;

        public static void Initialize() {
            dummyState = new KeyboardState();
            previousState = dummyState;
            currentState = Keyboard.GetState();

            KeyBinds.Load();
        }

        public static void Update() {
            previousState = currentState;
            currentState = Pigeon.IsInFocus ? Keyboard.GetState() : dummyState;

            foreach (var key in KeyBinds.BoundKeys) {
                if (IsPressed(key) && !Pigeon.Console.IsDisplaying) {
                    string boundCommand = KeyBinds.GetKeyBind(key);
                    Pigeon.Console.ExecuteCommand(boundCommand);
                }
            }
        }

        public static bool IsPressed(Keys key) {
            return currentState.IsKeyDown(key) && previousState.IsKeyUp(key);
        }

        public static bool IsPressed(params Keys[] keys) {
            return keys.Any(IsPressed);
        }

        public static bool IsPressed(ICollection<Keys> keys) {
            return keys.Any(IsPressed);
        }

        public static bool IsHeld(Keys key) {
            return currentState.IsKeyDown(key);
        }

        public static bool IsHeld(params Keys[] keys) {
            return keys.Any(key => currentState.IsKeyDown(key));
        }

        public static bool IsAnyKeyPressed() {
            return currentState.GetPressedKeys().Length > 0 && previousState.GetPressedKeys().Length == 0;
        }

        public static bool IsAnyKeyHeld() {
            return currentState.GetPressedKeys().Length > 0;
        }

        public static HashSet<Keys> GetJustPressedKeys() {
            var currentKeys = new HashSet<Keys>(currentState.GetPressedKeys());
            var previousKeys = new HashSet<Keys>(previousState.GetPressedKeys());

            var justPressedKeys = new HashSet<Keys>(currentKeys);

            foreach (var key in currentKeys) {
                if (previousKeys.Contains(key)) {
                    justPressedKeys.Remove(key);
                }
            }

            return justPressedKeys;
        }

        public static Keys GetSingleJustPressedKey() {
            var justPressedKeys = GetJustPressedKeys();
            return justPressedKeys.Count > 0 ? justPressedKeys.First() : Keys.None;
        }
    }
}
