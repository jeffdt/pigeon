using System.Collections.Generic;

namespace pigeon.input {
    public class InputReader {
        private readonly List<InputDevice> inputDevices = new List<InputDevice>();

        public static InputReader Combine(InputReader i1, InputReader i2) {
            InputReader combined = new InputReader();

            combined.inputDevices.AddRange(i1.inputDevices);
            combined.inputDevices.AddRange(i2.inputDevices);

            return combined;
        }

        public InputReader AddInputDevice(InputDevice inputDevice) {
            inputDevices.Add(inputDevice);
            return this;
        }

        public bool IsButtonPressed(int buttonNumber) {
            for (int i = 0; i < inputDevices.Count; i++) {
                if (inputDevices[i].IsButtonPressed(buttonNumber)) {
                    return true;
                }
            }

            return false;
        }

        public bool IsButtonPressed(int buttonNumber1, int buttonNumber2) {
            for (int i = 0; i < inputDevices.Count; i++) {
                var inputDevice = inputDevices[i];
                if (inputDevice.IsButtonPressed(buttonNumber1) || inputDevice.IsButtonPressed(buttonNumber2)) {
                    return true;
                }
            }

            return false;
        }

        public bool IsButtonPressed(int[] buttonNumbers) {
            for (int i = 0; i < inputDevices.Count; i++) {
                var inputDevice = inputDevices[i];
                for (int b = 0; b < buttonNumbers.Length; b++) {
                    if (inputDevice.IsButtonPressed(buttonNumbers[b])) {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsButtonHeld(int buttonNumber) {
            for (int i = 0; i < inputDevices.Count; i++) {
                if (inputDevices[i].IsButtonHeld(buttonNumber)) {
                    return true;
                }
            }

            return false;
        }

        public bool IsAnyPressed() {
            for (int i = 0; i < inputDevices.Count; i++) {
                if (inputDevices[i].IsAnyPressed()) {
                    return true;
                }
            }

            return false;
        }

        public bool IsAnyHeld() {
            for (int i = 0; i < inputDevices.Count; i++) {
                if (inputDevices[i].IsAnyHeld()) {
                    return true;
                }
            }

            return false;
        }
    }
}
