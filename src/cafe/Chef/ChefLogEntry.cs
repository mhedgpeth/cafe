using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NLog;
using LogLevel = NLog.LogLevel;

namespace cafe.Chef
{
    public class ChefLogEntry
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefLogEntry).FullName);

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
            {"FATAL", LogLevel.Fatal},
            {"INFO", LogLevel.Info},
            {"WARN", LogLevel.Warn}
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

        public static ChefLogEntry CreateMinimalEntry(string line)
        {
            return new ChefLogEntry(LogLevel.Info, null, line);
        }

        private static LogLevel ConvertToLogLevel(string lineValue)
        {
            try
            {
                return LevelMappings[ lineValue];
            }
            catch (KeyNotFoundException e)
            {
                Logger.Error(e, $"Could not convert {lineValue} into a valid level");
                throw;
            }
        }

        public void Log()
        {
            if (Level == LogLevel.Fatal)
                Logger.Fatal(Entry);
            else if (Level == LogLevel.Info)
                Logger.Info(Entry);
            else if (Level == LogLevel.Warn)
                Logger.Warn(Entry);
            else if (Level == LogLevel.Error)
                Logger.Error(Entry);
            else
                throw new InvalidOperationException($"Cannot log level {Level} because it is not supported");
        }

        public static ChefLogEntry CriticalError(string line)
        {
            return new ChefLogEntry(LogLevel.Info, null, line);
        }
    }
}