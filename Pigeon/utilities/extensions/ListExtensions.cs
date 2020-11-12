using System;
using System.Collections.Generic;

namespace pigeon.utilities.extensions {
    public static class ListExtensions {
        public static T Last<T>(this List<T> list) {
            return list[list.Count - 1];
        }

        public static Dictionary<K, V> ToDict<K, V>(this List<V> list, Func<V, K> keyFunction) {
            var dict = new Dictionary<K, V>();

            foreach (var element in list) {
                var key = keyFunction(element);
                dict.Add(key, element);
            }

            return dict;
        }
    }
}
