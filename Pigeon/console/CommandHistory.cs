using System.Collections.Generic;

namespace pigeon.console {
    internal class CommandHistory {
        private readonly int historyLength;
        private readonly List<string> commands;
        private int index;

        public CommandHistory(int historyLength) {
            this.historyLength = historyLength;
            commands = new List<string>(historyLength);
        }

        public void Commit(string command) {
            commands.Insert(0, command);

            while (commands.Count > historyLength) {
                commands.RemoveAt(commands.Count - 1);
            }

            index = -1;
        }

        public string Next() {
            if (commands.Count == 0) {
                return null;
            }

            if (index < commands.Count - 1) {
                index++;
            }

            return commands[index];
        }

        public string Previous() {
            if (commands.Count == 0) {
                return string.Empty;
            }

            if (index >= 0) {
                index--;
            }

            return index == -1 ? string.Empty : commands[index];
        }

        public void Reset() {
            index = -1;
        }
    }
}
