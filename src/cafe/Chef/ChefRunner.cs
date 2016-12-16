using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace cafe.Chef
{
    public class ChefRunner
    {
        private readonly Func<IChefProcess> _processCreator;

        public ChefRunner(Func<IChefProcess> processCreator)
        {
            _processCreator = processCreator;
        }

        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<ChefRunner>();

        public void Run()
        {
            var process = _processCreator();
            process.LogEntryReceived += (sender, entry) => entry.Log();
            process.Run();
        }

        public Version RetrieveVersion()
        {
            var process = _processCreator();
            Version version = null;
            process.LogEntryReceived += (sender, entry) =>
            {
                entry.Log();
                version = ParseVersion(entry.Entry);
            };
            process.Run("--version");
            return version;
        }

        public static Version ParseVersion(string entry)
        {
            var match = Regex.Match(entry, "Chef: (.*)");
            var versionString = match.Groups[1].Value;
            return Version.Parse(versionString);
        }
    }
}