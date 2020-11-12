using System;

namespace pigeon.utilities.extensions {
    public static class ArrayExtensions {
        public static T[] Slice<T>(this T[] data, int index) {
            return SubArray(data, index, data.Length - index);
        }

        public static T[] SubArray<T>(this T[] data, int index, int length) {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static void Populate<T>(this T[] arr, T value) {
            for (int i = 0; i < arr.Length; i++) {
                arr[i] = value;
            }
        }
    }
}
