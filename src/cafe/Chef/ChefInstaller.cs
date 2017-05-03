using System.IO;
using cafe.LocalSystem;
using cafe.Shared;
using NLog;

namespace cafe.Chef
{
    public interface IProductInstaller
    {
        Result Uninstall(string productCode);
        Result Install(string version);
    }

    public class ProductInstaller : IProductInstaller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ProductInstaller).FullName);

        private readonly IFileSystem _fileSystem;
        private readonly IFileSystemCommands _commands;
        private readonly string _installLocation;
        private readonly IDownloadUrlResolver _downloadUrlResolver;
        private readonly ProcessExecutor _processExecutor;

        public ProductInstaller(IFileSystem fileSystem, ProcessExecutor processExecutor, IFileSystemCommands commands,
            string installLocation, IDownloadUrlResolver downloadUrlResolver)
        {
            _fileSystem = fileSystem;
            _processExecutor = processExecutor;
            _commands = commands;
            _installLocation = installLocation;
            _downloadUrlResolver = downloadUrlResolver;
        }

        public Result Uninstall(string productCode)
        {
            Logger.Debug($"Uninstalling product {productCode}");
            var msiexec = FindFullPathToMsiExec();
            var result = _processExecutor.ExecuteAndWaitForExit(msiexec, $"/qn /x {productCode}", LogInformation, LogError);
            Logger.Debug($"Finished uninstalling {productCode} with result: {result}");
            return result;
        }

        public Result Install(string version)
        {
            Logger.Debug($"Installing version {version}");
            var fullPathToStagedInstaller = _downloadUrlResolver.FullPathToStagedInstaller(version);
            if (!_commands.FileExists(fullPathToStagedInstaller))
            {
                Logger.Warn(
                    $"No file for version {version} was staged at {fullPathToStagedInstaller}. Either download it or stage it another way");
                var failure = Result.Failure("There was no staged installer. Download the file first.");
                return failure;
            }
            Logger.Debug($"Installing installer {fullPathToStagedInstaller}");
            var msiexec = FindFullPathToMsiExec();
            var result = _processExecutor.ExecuteAndWaitForExit(msiexec, $"/qn /L*V \"logs/installation.log\" /i \"{fullPathToStagedInstaller}\" INSTALLLOCATION={_installLocation}",
                LogInformation, LogError);
            Logger.Debug($"Result of installing {fullPathToStagedInstaller} is {result}");
            return result;
        }

        private string FindFullPathToMsiExec()
        {
            const string msiexecExe = "msiexec.exe";
            var msiExecDirectory = _fileSystem.FindInstallationDirectoryInPathContaining(msiexecExe, @"C:\windows\System32");
            return Path.Combine(msiExecDirectory, msiexecExe);
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