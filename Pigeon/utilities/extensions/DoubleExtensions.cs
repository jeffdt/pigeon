namespace pigeon.utilities.extensions {
    public static class DoubleExtensions {
        public static double Clamp(this double value, double min, double max) {
            if (value < min) {
                return min;
            }

            return value > max ? max : value;
        }

        public static bool InRange(this double value, double min, double max) {
            return value >= min && value <= max;
        }
    }
}
