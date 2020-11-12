namespace pigeon.utilities.extensions {
    public static class BoolExtensions {
        public static float ToSign(this bool boolean) {
            return boolean ? 1f : -1f;
        }
    }
}
