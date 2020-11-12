using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace pigeon.input {
    public class InputManager {
        private readonly List<InputDevice> allDevices = new List<InputDevice>();

        private readonly List<GamepadInputDevice> gamepadDevices = new List<GamepadInputDevice>();
        private readonly List<KeyboardInputDevice> keyboardDevices = new List<KeyboardInputDevice>();

        private InputReader _defaultInputReader;

        public InputReader DefaultInputReader {
            get {
                if (_defaultInputReader == null) {
                    _defaultInputReader = new InputReader();
                    foreach (var keyboardDevice in keyboardDevices) {
                        _defaultInputReader.AddInputDevice(keyboardDevice);
                    }

                    foreach (var gamepad in gamepadDevices) {
                        _defaultInputReader.AddInputDevice(gamepad);
                    }
                }

                return _defaultInputReader;
            }
        }

        public void CreateGamepadReaders(Buttons[] buttonMap) {
            for (int i = 0; i < 4; i++) {
                GamepadInputDevice gamepadDevice = new GamepadInputDevice(i, buttonMap);
                gamepadDevices.Add(gamepadDevice);
                allDevices.Add(gamepadDevice);
            }
        }

        public void AddKeyboardReader(Keys[] keyMap) {
            KeyboardInputDevice keyboardDevice = new KeyboardInputDevice(keyMap);
            keyboardDevices.Add(keyboardDevice);
            allDevices.Add(keyboardDevice);
        }

        public void Update() {
            foreach (var inputReader in allDevices) {
                inputReader.Update();
            }
        }

        public InputDevice GetKeyboardDevice(int i) {
            return keyboardDevices[i];
        }

        public InputDevice GetGamepadDevice(int i) {
            return gamepadDevices[i];
        }
    }
}
