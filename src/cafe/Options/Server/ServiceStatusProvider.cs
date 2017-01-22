using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using cafe.LocalSystem;
using NLog;

namespace cafe.Options.Server
{
    public class ServiceStatusProvider
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(CafeWindowsServiceStatusOption).FullName);

        private readonly ProcessExecutor _processExecutor;
        private readonly IFileSystem _fileSystem;
        private readonly Regex _matchingState = new Regex(@"STATE\s+:\s(\d).*");
        private readonly IList<string> _cachedOutput = new List<string>();

        public static IDictionary<ServiceStatus, string> DescribeWindowsStatuses()
        {
            return new Dictionary<ServiceStatus, string>()
            {
                {ServiceStatus.Undetermined, "Undetermined"},
                {ServiceStatus.Stopped, "Stopped"},
                {ServiceStatus.IsStarting, "Is Starting"},
                {ServiceStatus.IsStopping, "Is Stopping"},
                {ServiceStatus.Running, "Running"},
                {ServiceStatus.ContinuePending, "Continue Pending"},
                {ServiceStatus.PausePending, "Pause Pending"},
                {ServiceStatus.Paused, "Paused"}
            };
        }

        private readonly IDictionary<ServiceStatus, string> _statusDescriptions = DescribeWindowsStatuses();

        public ServiceStatusProvider(ProcessExecutor processExecutor, IFileSystem fileSystem)
        {
            _processExecutor = processExecutor;
            _fileSystem = fileSystem;

        }

        public const string ServiceControllerExecutable = "sc.exe";

        public ServiceStatus DetermineStatus(string serviceName)
        {
            _cachedOutput.Clear();
            var fullPath = _fileSystem.FindInstallationDirectoryInPathContaining(ServiceControllerExecutable, @"C:\windows\System32");
            _processExecutor.ExecuteAndWaitForExit(Path.Combine(fullPath, ServiceControllerExecutable),
                $"query {serviceName}",
                CacheLog, LogError);
            return DetermineStateFromCachedOutput();
        }

        public string DetermineStatusDescription(string serviceName)
        {
            return _statusDescriptions[DetermineStatus(serviceName)];
        }

        private ServiceStatus DetermineStateFromCachedOutput()
        {
            foreach (var line in _cachedOutput.Where(s => !string.IsNullOrEmpty(s)))
            {
                Logger.Debug($"Determining if this line has the status: {line}");
                var match = _matchingState.Match(line);
                if (match.Success)
                {
                    Logger.Debug("matched, finding status");
                    var status = (ServiceStatus)Convert.ToInt32(match.Groups[1].Value);
                    Logger.Debug($"Match was status {status}");
                    return status;
                }
                else
                {
                    Logger.Debug($"Line {line} does not contain the status");
                }
            }
            return ServiceStatus.Undetermined;
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