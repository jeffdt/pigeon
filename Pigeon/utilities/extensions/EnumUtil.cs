﻿using System;
using System.Collections.Generic;

namespace pigeon.utilities.extensions {
    public static class EnumUtil {
        public static IEnumerable<T> GetValues<T>() {
            return (T[]) Enum.GetValues(typeof(T));
        }
    }
}
