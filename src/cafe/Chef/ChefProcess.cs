using System;
using System.Collections.Generic;
using System.IO;
using cafe.CommandLine;
using cafe.CommandLine.LocalSystem;
using cafe.LocalSystem;
using cafe.Shared;
using NLog;

namespace cafe.Chef
{
    public class ChefProcess : IChefProcess
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefProcess).FullName);

        private readonly ProcessExecutor _processExecutor;
        private readonly IFileSystem _fileSystem;


        public ChefProcess(ProcessExecutor processExecutor, IFileSystem fileSystem)
        {
            _processExecutor = processExecutor;
            _fileSystem = fileSystem;
        }

        public Result Run(params string[] args)
        {
            var binDirectory = _fileSystem.FindInstallationDirectoryInPathContaining("chef-client.bat",
                $@"{ServerSettings.Instance.InstallRoot}\opscode\chef\bin");
            var chefInstallDirectory = Directory.GetParent(binDirectory).FullName;
            var rubyExecutable = RubyExecutableWithin(chefInstallDirectory);
            var chefClientLoaderFile = ChefClientLoaderWithin(chefInstallDirectory);

            var arguments = new List<string>() {chefClientLoaderFile};
            arguments.AddRange(args);
            var processArguments = string.Join(" ", arguments);

            Logger.Info($"Running {rubyExecutable} with arguments: {processArguments}");

            string filename = rubyExecutable;
            EventHandler<string> processOnOutputDataReceived = ProcessOnOutputDataReceived;
            EventHandler<string> processOnErrorDataReceived = ProcessOnErrorDataReceived;
            return _processExecutor.ExecuteAndWaitForExit(filename, processArguments, processOnOutputDataReceived,
                processOnErrorDataReceived);
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
                Logger.Debug($"Chef error: {e}");
                OnLogEntryReceived(ChefLogEntry.CriticalError(e));
            }
        }

        private void ProcessOnOutputDataReceived(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
            {
                try
                {
                    Logger.Debug($"Chef info: {e}");
                    OnLogEntryReceived(ChefLogEntry.Parse(e));
                }
                catch (Exception exception)
                {
                    Logger.Fatal(exception, $"Could not parse and log {e}");
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