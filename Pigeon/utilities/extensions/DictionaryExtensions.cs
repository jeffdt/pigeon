using System;
using System.Collections.Generic;

namespace pigeon.utilities.extensions {
    public static class DictionaryExtensions {
        public static bool TryKey(this Dictionary<string, string> dictionary, string key, bool defaultValue) {
            if (!dictionary.ContainsKey(key)) {
                // TODO: log a statement when a parameter is defaulted for debugging purposes
                return defaultValue;
            }

            return Convert.ToBoolean(dictionary[key]);
        }

        public static int TryKey(this Dictionary<string, string> dictionary, string key, int defaultValue) {
            if (!dictionary.ContainsKey(key)) {
                // TODO: log a statement when a parameter is defaulted for debugging purposes
                return defaultValue;
            }

            return Convert.ToInt32(dictionary[key]);
        }

        public static float TryKey(this Dictionary<string, string> dictionary, string key, float defaultValue) {
            if (!dictionary.ContainsKey(key)) {
                // TODO: log a statement when a parameter is defaulted for debugging purposes
                return defaultValue;
            }

            return Convert.ToSingle(dictionary[key]);
        }
    }
}
