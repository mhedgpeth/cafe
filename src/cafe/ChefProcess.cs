using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace cafe
{
    public class ChefProcess : IChefProcess
    {
        private readonly Func<IProcess> _processCreator;
        private readonly IFileSystem _fileSystem;
        private readonly IEnvironment _environment;

        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<ChefProcess>();

        public ChefProcess(Func<IProcess> processCreator, IFileSystem fileSystem, IEnvironment environment)
        {
            _processCreator = processCreator;
            _fileSystem = fileSystem;
            _environment = environment;
        }

        public void Run(params string[] args)
        {
            var chefInstallDirectory = FindChefInstallationDirectory();
            var rubyExecutable = RubyExecutableWithin(chefInstallDirectory);
            var chefClientLoaderFile = ChefClientLoaderWithin(chefInstallDirectory);

            var arguments = new List<string>() {chefClientLoaderFile};
            arguments.AddRange(args);
            var processArguments = string.Join(" ", arguments);

            Logger.LogInformation($"Running {rubyExecutable} with arguments: {processArguments}");

            var process = _processCreator();
            process.StartInfo = new ProcessStartInfo(rubyExecutable)
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                Arguments = processArguments
            };
            process.OutputDataReceived += ProcessOnOutputDataReceived;
            process.ErrorDataReceived += ProcessOnErrorDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            Logger.LogInformation("Chef started; waiting for exit");
            process.WaitForExit();
            Logger.LogInformation($"Chef exited at {process.ExitTime} with status of {process.ExitCode}");
        }

        public event EventHandler<ChefLogEntry> LogEntryReceived;

        protected virtual void OnLogEntryReceived(ChefLogEntry e)
        {
            LogEntryReceived?.Invoke(this, e);
        }

        private void ProcessOnErrorDataReceived(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
            {
                OnLogEntryReceived(ChefLogEntry.CriticalError(e));
            }
        }

        private void ProcessOnOutputDataReceived(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
            {
                try
                {
                    OnLogEntryReceived(ChefLogEntry.Parse(e));
                }
                catch (Exception exception)
                {
                    Logger.LogCritical(default(EventId), exception, $"Could not parse and log {e}");
                }
            }
        }

        public string FindChefInstallationDirectory()
        {
            var environmentPath = _environment.GetEnvironmentVariable("PATH");
            var paths = environmentPath.Split(';');
            const string chefClientBat = "chef-client.bat";
            var batchFilePath = paths
                .Select(x => Path.Combine(x, chefClientBat))
                .FirstOrDefault(_fileSystem.FileExists);
            if (batchFilePath == null)
            {
                Logger.LogWarning($"Could not find {chefClientBat} in the path {environmentPath}");
                return null;
            }
            var binDirectory = Directory.GetParent(batchFilePath);
            var installDirectory = Directory.GetParent(binDirectory.FullName);
            return installDirectory.FullName;
        }

        public static string RubyExecutableWithin(string chefInstallPath)
        {
            return Path.Combine(chefInstallPath, @"embedded\bin\ruby.exe");
        }

        public static string ChefClientLoaderWithin(string chefInstallDirectory)
        {
            return $@"{chefInstallDirectory}\bin\chef-client";
        }
    }
}