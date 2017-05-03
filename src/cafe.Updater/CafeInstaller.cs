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

        public Result InstallOrUpgrade(string file)
        {
            var stopResult = DoCafeService("stop");
            if (stopResult.IsFailed)
            {
                Log.Error("Could not stop cafe service, so upgrade failed");
                return stopResult;
            }

            // extract to staged directory
            Logger.Info($"Extracting cafe installaction at {file} to {_cafeApplicationDirectory}");
            ZipFile.ExtractToDirectory(file, _cafeApplicationDirectory);

            var startResult = DoCafeService("start");
            if (startResult.IsFailed)
            {
                Log.Error("Could not restart cafe service, so upgrade failed");
                return startResult;
            }
            return Result.Successful();
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
            Logger.Info("Starting cafe service");
            Logger.Error(e);
        }

        private static void LogInformation(object sender, string e)
        {
            Logger.Info(e);
        }
    }
}