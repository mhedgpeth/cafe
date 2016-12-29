using System;
using System.IO;
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

        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AssemblyDirectory);
            ConfigureLogging();
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

        private static void ConfigureLogging()
        {
            LogManager.Configuration = new XmlLoggingConfiguration("nlog.config", false);
            Logger.Debug("Logging set up based on nlog.config");
        }

        public static Runner CreateRunner(string[] args)
        {
            var settings = ServerSettings.Read();
            var clientFactory = new ClientFactory(settings.Port);
            var schedulerWaiter = new SchedulerWaiter(clientFactory.RestClientForSchedulerServer, new AutoResetEventBoundary(), new TimerFactory(), new TaskStatusPresenter(new PresenterMessagePresenter()));
            var runner = new Runner(
                new RunChefOption(clientFactory, schedulerWaiter),
                new ShowChefVersionOption(clientFactory),
                new DownloadChefOption(clientFactory, schedulerWaiter),
                new InstallChefOption(clientFactory, schedulerWaiter),
                new ServerInteractiveOption(),
                new ServerWindowsServiceOption(),
                new RegisterServerWindowsServiceOption(),
                new UnregisterServerWindowsServiceOption(),
                new CafeWindowsServiceStatusOption(new ProcessExecutor(() => new ProcessBoundary()), new FileSystem(new EnvironmentBoundary(), new FileSystemCommandsBoundary())),
                new StatusOption(clientFactory.RestClientForSchedulerServer),
                new ShowChefStatusOption(clientFactory.RestClientForSchedulerServer),
                ChangeChefRunningStatusOption.CreatePauseChefOption(clientFactory.RestClientForSchedulerServer),
                ChangeChefRunningStatusOption.CreateResumeChefOption(clientFactory.RestClientForSchedulerServer));
            Logger.Debug("Running application");
            return runner;
        }
    }
}