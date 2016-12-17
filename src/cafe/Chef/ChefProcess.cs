using System;
using System.Collections.Generic;
using System.IO;
using cafe.LocalSystem;
using Microsoft.Extensions.Logging;

namespace cafe.Chef
{
    public class ChefProcess : IChefProcess
    {
        private readonly ProcessExecutor _processExecutor;
        private readonly IFileSystem _fileSystem;

        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<ChefProcess>();

        public ChefProcess(ProcessExecutor processExecutor, IFileSystem fileSystem)
        {
            _processExecutor = processExecutor;
            _fileSystem = fileSystem;
        }

        public void Run(params string[] args)
        {
            var binDirectory = _fileSystem.FindInstallationDirectoryInPathContaining("chef-client.bat");
            var chefInstallDirectory = Directory.GetParent(binDirectory).FullName;
            var rubyExecutable = RubyExecutableWithin(chefInstallDirectory);
            var chefClientLoaderFile = ChefClientLoaderWithin(chefInstallDirectory);

            var arguments = new List<string>() {chefClientLoaderFile};
            arguments.AddRange(args);
            var processArguments = string.Join(" ", arguments);

            Logger.LogInformation($"Running {rubyExecutable} with arguments: {processArguments}");

            string filename = rubyExecutable;
            EventHandler<string> processOnOutputDataReceived = ProcessOnOutputDataReceived;
            EventHandler<string> processOnErrorDataReceived = ProcessOnErrorDataReceived;
            _processExecutor.ExecuteAndWaitForExit(filename, processArguments, processOnOutputDataReceived, processOnErrorDataReceived);
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