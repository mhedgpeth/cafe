using System;
using System.IO;
using System.Reflection;
using cafe.CommandLine;
using cafe.CommandLine.LocalSystem;
using cafe.Shared;
using NLog;

namespace cafe.Chef
{
    public class CafeInstaller : IInstaller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(CafeInstaller).FullName);

        private readonly IFileSystemCommands _commands;
        private readonly IDownloadUrlResolver _resolver;
        private readonly string _updaterDirectory;

        public CafeInstaller(IFileSystemCommands commands, IDownloadUrlResolver resolver, string updaterDirectory)
        {
            _commands = commands;
            _resolver = resolver;
            _updaterDirectory = updaterDirectory;
        }

        public Version InstalledVersion => Assembly.GetEntryAssembly().GetName().Version;
        public string ProductName => "cafe";


        public Result InstallOrUpgrade(string version, IMessagePresenter presenter)
        {
            string cafeStaging = Path.Combine(_updaterDirectory, "staging");
            var installer = _resolver.FullPathToStagedInstaller(version);
            if (!_commands.FileExists(installer))
            {
                Logger.Error("File is not staged at {installer}. Run download first");
                return Result.Failure(
                    $"Could not install cafe {version} because there is no installer staged at {installer}. Run download first.");
            }
            Logger.Info($"Copying {installer} to {cafeStaging} to be processed by the updater");
            File.Copy(installer, Path.Combine(cafeStaging, Path.GetFileName(installer)));

            Logger.Info($"This service will stop soon and be updated to version {version}");
            return Result.Successful();
        }
    }
}