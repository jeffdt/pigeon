using pigeon;
using Serilog.Core;
using Serilog.Events;
using System;

namespace PigeonEngine.logger {
    public class PGNConsoleSink : ILogEventSink {
        private readonly IFormatProvider _formatProvider;

        public PGNConsoleSink(IFormatProvider formatProvider) {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent) {
            var message = logEvent.RenderMessage(null);
            Pigeon.Console.Log(DateTimeOffset.Now.ToString("hh:mm:ss") + " " + message);
        }
    }
}
