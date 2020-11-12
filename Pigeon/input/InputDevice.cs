namespace pigeon.input {
    public abstract class InputDevice {
        public abstract void Update();

        public abstract bool IsButtonPressed(int buttonNumber);
        public abstract bool IsButtonHeld(int buttonNumber);

        public abstract bool IsAnyPressed();
        public abstract bool IsAnyHeld();
    }
}
