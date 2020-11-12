using System.Collections.Generic;
using System.Text;

namespace pigeon.console {
    public static class ConsoleUtilities {
        public static string BracketedList(IEnumerable<string> strings) {
            var builder = new StringBuilder();
            foreach (var entry in strings) {
                builder.Append("[");
                builder.Append(entry);
                builder.Append("] ");
            }

            return builder.ToString();
        }

        public static void LogVariableChange(string variableName, object oldValue, object newValue) {
            Pigeon.Console.Log(string.Format("{0}:{1}`{2}", variableName, oldValue, newValue));
        }

        public static void LogVariable(string variableName, object value) {
            Pigeon.Console.Log(string.Format("{0}: {1}", variableName, value));
        }
    }
}
