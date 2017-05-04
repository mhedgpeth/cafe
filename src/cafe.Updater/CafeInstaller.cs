using System;
using System.IO;
using System.IO.Compression;
using cafe.CommandLine;
using cafe.CommandLine.LocalSystem;
using NLog;
using NLog.Fluent;

namespace cafe.Updater
{
    public class CafeInstaller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(CafeInstaller).FullName);

        private readonly IFileSystemCommands _commands;
        private readonly ProcessExecutor _processExecutor;
        private readonly string _cafeApplicationDirectory;

        public CafeInstaller(IFileSystemCommands commands, ProcessExecutor processExecutor, string cafeApplicationDirectory)
        {
            _commands = commands;
            _processExecutor = processExecutor;
            _cafeApplicationDirectory = cafeApplicationDirectory;
        }

        private const string StagingExtractedDirectory = "staging-extracted";

        public Result InstallOrUpgrade(string file)
        {
            try
            {
                Logger.Info($"Deleting extracted directory {StagingExtractedDirectory} if it exists");
                if (_commands.DirectoryExists(StagingExtractedDirectory))
                {
                    _commands.DeleteDirectory(StagingExtractedDirectory);
                }
                Logger.Debug($"Creating staging extracted directory {StagingExtractedDirectory}");
                _commands.CreateDirectory(StagingExtractedDirectory);

                Logger.Info("Stopping cafe service so an upgrade can occur.");
                var stopResult = DoCafeService("stop");
                if (stopResult.IsFailed)
                {
                    Logger.Error("Could not stop cafe service, so upgrade failed");
                    return stopResult;
                }

                // extract to staged directory
                Logger.Info($"Extracting cafe installaction at {file} to {StagingExtractedDirectory}");
                ZipFile.ExtractToDirectory(file, StagingExtractedDirectory);

                Logger.Info($"Copying all files to cafe install directory at {_cafeApplicationDirectory}");
                foreach (var applicationFile in Directory.GetFiles(StagingExtractedDirectory, "*.*", SearchOption.TopDirectoryOnly))
                {
                    if (applicationFile.Contains("server.json"))
                    {
                        Logger.Debug("Not copying server.json");
                        continue;
                    }
                    Logger.Info($"Copying {applicationFile} to cafe application directory at {_cafeApplicationDirectory}");
                    File.Copy(applicationFile, Path.Combine(_cafeApplicationDirectory, Path.GetFileName(applicationFile)), true);
                }

                Logger.Info("Finished copying files. Starting service");
                var startResult = DoCafeService("start");
                if (startResult.IsFailed)
                {
                    Logger.Error("Could not restart cafe service, so upgrade failed");
                    return startResult;
                }
                Logger.Info("Finished starting service. Upgrade was successful.");
                return Result.Successful();

            }
            catch (Exception e)
            {
                Logger.Error($"Unexpected exception while upgrading {file}: {e}");
                return Result.Failure("Unexpected exception while upgrading");
            }
        }

        private void StopCafeService()
        {
            Logger.Info("Stopping cafe service");
            DoCafeService("stop");
        }

        private Result DoCafeService(string verb)
        {
            return _processExecutor.ExecuteAndWaitForExit(Path.Combine(_cafeApplicationDirectory, "cafe.exe"), $"service {verb}",
                LogInformation, LogError);
        }

        private static void LogError(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
            {
                Logger.Error(e);
            }
        }

        private static void LogInformation(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
            {
                Logger.Info(e);
            }
        }
    }
}