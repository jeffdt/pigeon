namespace pigeon.console {
    internal class LogMessage {
        public string Text;
        public LogMessageTypes Type;

        public LogMessage(string text, LogMessageTypes type = LogMessageTypes.Info) {
            Text = text;
            Type = type;
        }
    }
}
