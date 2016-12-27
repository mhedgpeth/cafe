using cafe.Chef;
using cafe.Client;
using cafe.CommandLine;
using cafe.LocalSystem;
using cafe.Options;
using NLog;
using NLog.Config;

namespace cafe
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Program).FullName);

        public static void Main(string[] args)
        {
            ConfigureLogging();
            Presenter.ShowApplicationHeading(Logger, args);
            var runner = CreateRunner(args);
            runner.Run(args);
            Logger.Debug("Finishing cafe run");
        }

        private static void ConfigureLogging()
        {
            LogManager.Configuration = new XmlLoggingConfiguration("nlog.config", false);
            Logger.Debug("Logging set up based on nlog.config");
        }

        public static Runner CreateRunner(string[] args)
        {
            Logger.Debug("Creating chef runner");
            var chefRunner = CreateChefRunner();
            Logger.Debug("Creating runner");
            var runner = new Runner(
                new RunChefOption(chefRunner),
                new ShowChefVersionOption(new ClientFactory()),
                new DownloadChefOption(new ChefDownloader(new FileDownloader(),
                    new FileSystem(new EnvironmentBoundary(), new FileSystemCommandsBoundary()))),
                new InstallChefOption(new ChefInstaller(CreateFileSystem(), CreateProcessExecutor(),
                    new FileSystemCommandsBoundary())),
                new ServerOption(),
                new SchedulerStatusOption(new ClientFactory()));
            Logger.Debug("Running application");
            return runner;
        }

        private static ChefRunner CreateChefRunner()
        {
            return
                new ChefRunner(
                    () =>
                        new ChefProcess(CreateProcessExecutor(),
                            CreateFileSystem()));
        }

        private static ProcessExecutor CreateProcessExecutor()
        {
            return new ProcessExecutor(() => new ProcessBoundary());
        }

        private static FileSystem CreateFileSystem()
        {
            return new FileSystem(new EnvironmentBoundary(), new FileSystemCommandsBoundary());
        }
    }
}