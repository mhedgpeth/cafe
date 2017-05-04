using System.IO;
using DasMulli.Win32.ServiceUtils;
using NLog;

namespace cafe.Updater
{
    public class CafeUpdaterWindowsService : IWin32Service
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Program).FullName);

        private readonly CafeInstaller _cafeInstaller;

        public CafeUpdaterWindowsService(CafeInstaller cafeInstaller)
        {
            _cafeInstaller = cafeInstaller;
        }

        private FileSystemWatcher _watcher;
        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            const string staging = "staging";
            if (!Directory.Exists(staging))
            {
                Logger.Info($"Creating staging directory: {Path.GetFullPath(staging)}");
                Directory.CreateDirectory(staging);
            }
            else
            {
                Logger.Info($"Since staging directory already exists at {Path.GetFullPath(staging)}, not creating it");
            }
            const string filter = "cafe-*.zip";
            Logger.Info($"Setting up file watcher to watch for {filter}");
            _watcher = new FileSystemWatcher(staging) {Filter = filter};
            _watcher.Created += WatcherOnCreated;
            _watcher.Deleted += WatcherOnDeleted;

            Logger.Debug("Watching for files");
            _watcher.EnableRaisingEvents = true;
        }

        private void WatcherOnDeleted(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Logger.Info($"File {fileSystemEventArgs.FullPath} was deleted");
        }

        private void WatcherOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            var installerFile = fileSystemEventArgs.FullPath;
            Logger.Info($"File {installerFile} was created");
            var result = _cafeInstaller.InstallOrUpgrade(installerFile);
            Logger.Info($"Result of installation was: {result}");
            Logger.Info("Removing staged file");
            File.Delete(installerFile);
            Logger.Info("Installer file removed");
        }

        public void Stop()
        {
            if (_watcher != null)
            {
                Logger.Info("Stopping watching for files");
                _watcher.EnableRaisingEvents = false;
            }
        }

        public string ServiceName => "cafe.Updater";
    }
}