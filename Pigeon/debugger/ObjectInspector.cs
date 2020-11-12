using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using pigeon.utilities;

namespace pigeon.debugger {
    public class ObjectInspector2 {
        private readonly List<Tuple<string, string>> fields = new List<Tuple<string, string>>();

        public void AppendField(string name, object value) {
            var field = new Tuple<string, string>(name, describeValue(value));
            fields.Add(field);
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder();

            for (int j = 0; j < fields.Count; j++) {
                var field = fields[j];

                if (j > 0) {
                    builder.Append(",");
                }

                builder.Append(field.Item1);
                builder.Append("=");
                builder.Append(field.Item2);
            }

            return builder.ToString();
        }

        private static string describeValue(object value) {
            if (value is Vector2 vector2) {
                var casted = vector2;
                return casted.ToFormattedString();
            }

            if (value is Point point) {
                var casted = point;
                return casted.ToFormattedString();
            }
            return value.ToString();
        }
    }
}
