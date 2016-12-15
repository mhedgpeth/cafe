using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace cafe
{
    public class ChefLogEntry
    {
        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<ChefLogEntry>();

        private readonly LogLevel _level;
        private readonly DateTime? _time;
        private readonly string _entry;

        public LogLevel Level => _level;
        public DateTime? Time => _time;
        public string Entry => _entry;

        private ChefLogEntry(LogLevel level, DateTime? time, string entry)
        {
            _level = level;
            _time = time;
            _entry = entry;
        }

        private static readonly IDictionary<string, LogLevel> LevelMappings = new Dictionary<string, LogLevel>
        {
            {"ERROR", LogLevel.Error},
            {"FATAL", LogLevel.Critical},
            {"INFO", LogLevel.Information},
            {"WARN", LogLevel.Warning}
        };

        public static ChefLogEntry Parse(string line)
        {
            if (string.IsNullOrEmpty(line)) return CreateMinimalEntry(string.Empty);
            if (!line.StartsWith("[")) return CreateMinimalEntry(line);

            var match = Regex.Match(line, @"\[([^\]]+)\]\s([A-Z]+):\s(.*)");
            var time = DateTime.Parse(match.Groups[1].Value);
            var lineValue = match.Groups[2].Value;
            var level = ConvertToLogLevel(lineValue);
            var entry = match.Groups[3].Value;

            return new ChefLogEntry(level, time, entry);
        }

        private static ChefLogEntry CreateMinimalEntry(string line)
        {
            return new ChefLogEntry(LogLevel.Information, null, line);
        }

        private static LogLevel ConvertToLogLevel(string lineValue)
        {
            try
            {
                return LevelMappings[ lineValue];
            }
            catch (KeyNotFoundException e)
            {
                Logger.LogCritical(default(EventId), e, $"Could not convert {lineValue} into a valid level");
                throw;
            }
        }

        public void Log()
        {
            switch (Level)
            {
                case LogLevel.Critical:
                    Logger.LogCritical(Entry);
                    break;
                case LogLevel.Information:
                    Logger.LogInformation(Entry);
                    break;
                case LogLevel.Warning:
                    Logger.LogWarning(Entry);
                    break;
                case LogLevel.Error:
                    Logger.LogError(Entry);
                    break;
                default:
                    throw new InvalidOperationException($"Cannot log level {Level} because it is not supported");
            }
        }
    }
}