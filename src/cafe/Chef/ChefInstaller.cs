using cafe.LocalSystem;
using NLog;

namespace cafe.Chef
{
    public class ChefInstaller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IFileSystem _fileSystem;
        private readonly ProcessExecutor _processExecutor;
        private readonly IFileSystemCommands _commands;

        public ChefInstaller(IFileSystem fileSystem, ProcessExecutor processExecutor, IFileSystemCommands commands)
        {
            _fileSystem = fileSystem;
            _processExecutor = processExecutor;
            _commands = commands;
        }

        public void InstallOrUpgrade(string version)
        {
            var fullPathToStagedInstaller = ChefDownloader.FullPathToStagedInstaller(version);
            if (!_commands.FileExists(fullPathToStagedInstaller))
            {
                Logger.Warn(
                    $"No file for version {version} was staged at {fullPathToStagedInstaller}. Either download it or stage it another way");
            }

            var msiExecDirectory = _fileSystem.FindInstallationDirectoryInPathContaining("msiexec.exe");
            _processExecutor.ExecuteAndWaitForExit(msiExecDirectory, $"/qn /i \"{fullPathToStagedInstaller}\"",
                LogInformation, LogError);
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