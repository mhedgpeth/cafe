using System;
using System.IO;
//using System.IO.Compression;
using System.Reflection;
using cafe.LocalSystem;
using cafe.Shared;
using NLog;

namespace cafe.Chef
{
    public class CafeInstaller : IInstaller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(CafeInstaller).FullName);

        private readonly IFileSystemCommands _commands;
        private readonly IDownloadUrlResolver _resolver;
        private readonly ProcessExecutor _processExecutor;

        public CafeInstaller(IFileSystemCommands commands, IDownloadUrlResolver resolver, ProcessExecutor processExecutor)
        {
            _commands = commands;
            _resolver = resolver;
            _processExecutor = processExecutor;
        }

        public Version InstalledVersion => Assembly.GetEntryAssembly().GetName().Version;
        public string ProductName => "cafe";


        public Result InstallOrUpgrade(string version, IMessagePresenter presenter)
        {
            const string cafeStaging = "cafe-staging";
            if (_commands.DirectoryExists(cafeStaging))
            {
                _commands.DeleteDirectory(cafeStaging);
            }
//            ZipFile.ExtractToDirectory(_resolver.FullPathToStagedInstaller(version), cafeStaging);
            var upgradeBatchFile = Path.GetFullPath("upgrade.bat");
            var startTime = DateTime.Now.AddMinutes(1).ToString("HH:mm");
            // command SchTasks.exe /SC ONCE /TN Upgrade-Cafe-{version} /TR {upgradeBatchFile} /ST {startTime} /F
            // later: /Z to mark it for deletion
            _processExecutor.ExecuteAndWaitForExit($"C:\\system32\\SchTasks.exe",
                "/SC ONCE /TN Upgrade-Cafe-{version} /TR {upgradeBatchFile} /ST {startTime} /F", LogInformation,
                LogError);
            Logger.Warn($"Created scheduled task for {startTime} to upgrade cafe to {version}. Cafe will restart and should not be running any tasks.");
            return Result.Successful();
        }

        private static void LogError(object sender, string e)
        {
            Logger.Error(e);
        }

        private static void LogInformation(object sender, string e)
        {
            Logger.Info(e);
        }
    }
}