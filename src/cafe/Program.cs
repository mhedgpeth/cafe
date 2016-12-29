using System;
using System.IO;
using System.Linq;
using System.Reflection;
using cafe.Client;
using cafe.CommandLine;
using cafe.LocalSystem;
using cafe.Options;
using cafe.Options.Server;
using cafe.Server.Scheduling;
using NLog;
using NLog.Config;

namespace cafe
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Program).FullName);
        public const string ServerLoggingConfigurationFile = "nlog-server.config";
        public const string ClientLoggingConfigurationFile = "nlog-client.config";

        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AssemblyDirectory);
            ConfigureLogging(args);
            Presenter.ShowApplicationHeading(Logger, args);
            var runner = CreateRunner(args);
            runner.Run(args);
            Logger.Debug("Finishing cafe run");
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetEntryAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private static void ConfigureLogging(params string[] args)
        {
            var file = LoggingConfigurationFileFor(args);
            LogManager.Configuration = new XmlLoggingConfiguration(file, false);
            Logger.Info($"Logging set up based on {file}");
        }

        public static Runner CreateRunner(string[] args)
        {
            var settings = ServerSettings.Read();
            var clientFactory = new ClientFactory(settings.Port);
            var schedulerWaiter = new SchedulerWaiter(clientFactory.RestClientForSchedulerServer, new AutoResetEventBoundary(), new TimerFactory(), new TaskStatusPresenter(new PresenterMessagePresenter()));
            var processExecutor = new ProcessExecutor(() => new ProcessBoundary());
            var environment = new EnvironmentBoundary();
            var fileSystem = new FileSystem(environment, new FileSystemCommandsBoundary());
            var serviceStatusWaiter = new ServiceStatusWaiter("waiting for service status", new AutoResetEventBoundary(), new TimerFactory(), new ServiceStatusProvider(processExecutor, fileSystem));
            var runner = new Runner(
                new RunChefOption(clientFactory, schedulerWaiter),
                new ShowChefVersionOption(clientFactory),
                new DownloadChefOption(clientFactory, schedulerWaiter),
                new InstallChefOption(clientFactory, schedulerWaiter),
                new ServerInteractiveOption(),
                new ServerWindowsServiceOption(),
                new RegisterServerWindowsServiceOption(),
                new UnregisterServerWindowsServiceOption(),
                ChangeStateForCafeWindowsServiceOption.StartCafeWindowsServiceOption(processExecutor, fileSystem, serviceStatusWaiter),
                ChangeStateForCafeWindowsServiceOption.StopCafeWindowsServiceOption(processExecutor, fileSystem, serviceStatusWaiter),
                new CafeWindowsServiceStatusOption(processExecutor, fileSystem),
                new StatusOption(clientFactory.RestClientForSchedulerServer),
                new ShowChefStatusOption(clientFactory.RestClientForSchedulerServer),
                ChangeChefRunningStatusOption.CreatePauseChefOption(clientFactory.RestClientForSchedulerServer),
                ChangeChefRunningStatusOption.CreateResumeChefOption(clientFactory.RestClientForSchedulerServer),
                new InitOption(AssemblyDirectory, environment));
            Logger.Debug("Running application");
            return runner;
        }

        public static string LoggingConfigurationFileFor(string[] args)
        {
            return args.FirstOrDefault() == "server" ? ServerLoggingConfigurationFile : ClientLoggingConfigurationFile;
        }
    }
}