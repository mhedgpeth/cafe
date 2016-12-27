using cafe.LocalSystem;
using cafe.Server.Scheduling;
using cafe.Shared;
using NLog;

namespace cafe.Chef
{
    public class ChefInstaller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefInstaller).FullName);

        private readonly IFileSystem _fileSystem;
        private readonly ProcessExecutor _processExecutor;
        private readonly IFileSystemCommands _commands;

        public ChefInstaller(IFileSystem fileSystem, ProcessExecutor processExecutor, IFileSystemCommands commands)
        {
            _fileSystem = fileSystem;
            _processExecutor = processExecutor;
            _commands = commands;
        }

        public Result InstallOrUpgrade(string version, IMessagePresenter presenter)
        {
            presenter.ShowMessage($"Installing/Upgrading Chef to version {version}");
            var fullPathToStagedInstaller = ChefDownloader.FullPathToStagedInstaller(version);
            if (!_commands.FileExists(fullPathToStagedInstaller))
            {
                Logger.Warn(
                    $"No file for version {version} was staged at {fullPathToStagedInstaller}. Either download it or stage it another way");
                var failure = Result.Failure("There was no staged installer. Download the file first.");
                presenter.ShowMessage(failure.ToString());
                return failure;
            }

            presenter.ShowMessage("Running installer");
            var msiExecDirectory = _fileSystem.FindInstallationDirectoryInPathContaining("msiexec.exe");
            var result = _processExecutor.ExecuteAndWaitForExit(msiExecDirectory, $"/qn /i \"{fullPathToStagedInstaller}\"",
                LogInformation, LogError);
            presenter.ShowMessage($"Result of installing chef {version} was: {result}");
            return result;
        }

        private void LogError(object sender, string e)
        {
            Logger.Error(e);
        }

        private void LogInformation(object sender, string e)
        {
            Logger.Info(e);
        }
    }
}