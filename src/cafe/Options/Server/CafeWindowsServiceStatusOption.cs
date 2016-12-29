using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using cafe.CommandLine;
using cafe.LocalSystem;
using cafe.Shared;
using NLog;
using NLog.Fluent;

namespace cafe.Options.Server
{
    public class CafeWindowsServiceStatusOption : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(CafeWindowsServiceStatusOption).FullName);

        private readonly ProcessExecutor _processExecutor;
        private readonly IFileSystem _fileSystem;
        private readonly Regex _matchingState = new Regex(@"STATE\s+:\s(\d).*");
        private readonly IList<string> _cachedOutput = new List<string>();

        private readonly IDictionary<int, string> _statusDescriptions = new Dictionary<int, string>()
        {
            {0, "Undetermined"},
            {1, "Stopped"},
            {2, "Is Starting"},
            {3, "Is Stopping"},
            {4, "Running"},
            {5, "Continue Pending"},
            {6, "Pause Pending"},
            {7, "Paused"}
        };

        public CafeWindowsServiceStatusOption(ProcessExecutor processExecutor, IFileSystem fileSystem)
            : base(new OptionSpecification("service", "status"), "gets the status of the cafe windows service")
        {
            _processExecutor = processExecutor;
            _fileSystem = fileSystem;
        }

        protected override string ToDescription(string[] args)
        {
            return "Determining Cafe Windows Service Status";
        }

        protected override Result RunCore(string[] args)
        {
            _cachedOutput.Clear();
            var executable = "sc.exe";
            var fullPath = _fileSystem.FindInstallationDirectoryInPathContaining(executable);
            var serviceName = CafeServerWindowsServiceOptions.ServiceName;
            _processExecutor.ExecuteAndWaitForExit(Path.Combine(fullPath, executable),
                $"query {serviceName}",
                CacheLog, LogError);
            var status = DetermineState();
            Presenter.ShowMessage($"{serviceName} status is {status}", Logger);
            return Result.Successful();
        }

        private string DetermineState()
        {
            foreach (var line in _cachedOutput)
            {
                Logger.Debug($"Determining if this line has the status: {line}");
                var match = _matchingState.Match(line);
                if (match.Success)
                {
                    Logger.Debug("matched, finding status");
                    var status = Convert.ToInt32(match.Groups[1].Value);
                    Logger.Debug($"Match was status {status}");
                    return _statusDescriptions[status];
                }
                else
                {
                    Logger.Debug($"Line {line} does not contain the status");
                }
            }
            return "Undetermined";
        }

        private void CacheLog(object sender, string e)
        {
            _cachedOutput.Add(e);
            Logger.Debug(e);
        }

        private static void LogError(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
            {
                Logger.Error(e);
            }
        }
    }
}