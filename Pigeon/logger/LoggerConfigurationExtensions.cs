using Serilog;
using Serilog.Configuration;
using System;

namespace PigeonEngine.logger {
    public static class LoggerConfigurationExtensions {
        public static LoggerConfiguration PGNConsole(
              this LoggerSinkConfiguration loggerConfiguration,
              IFormatProvider formatProvider = null) {
            return loggerConfiguration.Sink(new PGNConsoleSink(formatProvider));
        }
    }
}
