using System;
using System.Text.RegularExpressions;
using cafe.Server.Scheduling;
using cafe.Shared;
using NLog;

namespace cafe.Chef
{
    public class ChefRunner
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefRunner).FullName);

        private readonly Func<IChefProcess> _processCreator;

        public ChefRunner(Func<IChefProcess> processCreator)
        {
            _processCreator = processCreator;
        }


        public Result Run(IMessagePresenter presenter)
        {
            presenter.ShowMessage("Running chef");
            var process = _processCreator();
            process.LogEntryReceived += (sender, entry) =>
            {
                presenter.ShowMessage(entry.Entry);
                entry.Log();
            };
            var result = process.Run();
            presenter.ShowMessage($"Finished running chef with result: {result}");
            return result;
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