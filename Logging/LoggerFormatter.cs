using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace TianyiNetwork.Web.AppsApi.Logging
{
    public class LoggerFormatter : ConsoleFormatter, IDisposable
    {
        private readonly IDisposable? _optionsReloadToken;
        private LoggerOptions _options;

        public LoggerFormatter(IOptionsMonitor<LoggerOptions> options) : base("localized")
        {
            _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
            _options = options.CurrentValue;
        }

        private void ReloadLoggerOptions(LoggerOptions options) => _options = options;

        public override void Write<TState>(in LogEntry<TState> entry, IExternalScopeProvider? scope, TextWriter writer)
        {
            // ReSharper disable once StringLiteralTypo
            writer.Write(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz "));
            writer.Write(GetLogLevelMessage(entry.LogLevel));
            writer.Write($"<\x1b[36m{entry.Category}\x1b[00m> ");

            var message = entry.Formatter?.Invoke(entry.State, entry.Exception) ?? string.Empty;
            var messageLines = message.Split("\n");
            writer.WriteLine(messageLines.FirstOrDefault(string.Empty));
            messageLines
                .Skip(1)
                .Select(x => new string(' ', _options.MultiLinePrefixLength) + x)
                .ToList()
                .ForEach(writer.WriteLine);
        }

        public static string GetLogLevelMessage(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "[\x1b[01;39mTRACE\x1b[0m]";
                case LogLevel.Debug: 
                    return "[\x1b[01;34mDEBUG\x1b[0m]";
                case LogLevel.Information:
                    return "[\x1b[01;32mINFO\x1b[0m] ";
                case LogLevel.Warning:
                    return "[\x1b[01;33mWARN\x1b[0m] ";
                case LogLevel.Error:
                    return "[\x1b[01;31mERROR\x1b[0m]";
                case LogLevel.Critical:
                    // ReSharper disable once StringLiteralTypo
                    return "[\x1b[01;35mCRIT\x1b[0m] ";
                default:
                    return "[\x1b[01;37mNONE\x1b[0m] ";
            }
        }

        public void Dispose() => _optionsReloadToken?.Dispose();
    }
}
