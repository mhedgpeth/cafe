using System;
using System.IO;
using System.Linq;
using System.Reflection;
using cafe.Client;
using cafe.CommandLine;
using cafe.LocalSystem;
using cafe.Options;
using cafe.Options.Chef;
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
        private const string ClientLoggingConfigurationFile = "nlog-client.config";

        public static int Main(string[] args)
        {
            Directory.SetCurrentDirectory(AssemblyDirectory);
            ConfigureLogging(args);
            Presenter.ShowApplicationHeading(Logger, args);
            var runner = CreateRunner(args);
            var returnValue = runner.RunProgram(args);
            Logger.Debug("Finishing cafe run");
            return returnValue;
        }

        private static string AssemblyDirectory
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

        private static OptionGroup CreateRunner(string[] args)
        {
            var clientFactory = new ClientFactory(ClientSettings.Instance.Node, ClientSettings.Instance.Port);
            var schedulerWaiter = new SchedulerWaiter(clientFactory.RestClientForJobServer,
                new AutoResetEventBoundary(), new TimerFactory(),
                new JobRunStatusPresenter(new PresenterMessagePresenter()));
            var processExecutor = new ProcessExecutor(() => new ProcessBoundary());
            var environment = new EnvironmentBoundary();
            var fileSystemCommands = new FileSystemCommandsBoundary();
            var fileSystem = new FileSystem(environment, fileSystemCommands);
            var serviceStatusWaiter = new ServiceStatusWaiter("waiting for service status",
                new AutoResetEventBoundary(), new TimerFactory(),
                new ServiceStatusProvider(processExecutor, fileSystem));
            // all options available

            var root = new OptionGroup()
                .WithGroup("chef", chefGroup =>
                {
                    chefGroup.WithOption(new RunChefOption(clientFactory.RestClientForChefServer, schedulerWaiter), "run");
                    chefGroup.WithOption(new DownloadChefOption(clientFactory.RestClientForChefServer, schedulerWaiter),
                        OptionValueSpecification.ForExactValue("download"),
                        OptionValueSpecification.ForVersion());
                    chefGroup.WithOption(new ShowChefStatusOption(clientFactory), "status");
                    chefGroup.WithOption(
                        new BootstrapChefRunListOption(clientFactory.RestClientForChefServer, schedulerWaiter, fileSystemCommands),
                        OptionValueSpecification.ForExactValue("bootstrap"),
                        OptionValueSpecification.ForExactValue("run-list:"),
                        OptionValueSpecification.ForAnyValue("the run list"),
                        OptionValueSpecification.ForExactValue("config:"),
                        OptionValueSpecification.ForAnyValue("the client.rb file"),
                        OptionValueSpecification.ForExactValue("validator:"),
                        OptionValueSpecification.ForAnyValue("the validator.pem file used to join the node"));
                    chefGroup.WithOption(
                        new BootstrapChefPolicyOption(clientFactory.RestClientForChefServer, schedulerWaiter, fileSystemCommands),
                        OptionValueSpecification.ForExactValue("bootstrap"),
                        OptionValueSpecification.ForExactValue("policy:"),
                        OptionValueSpecification.ForAnyValue("the policy name"),
                        OptionValueSpecification.ForExactValue("group:"),
                        OptionValueSpecification.ForAnyValue("the policy group"),
                        OptionValueSpecification.ForExactValue("config:"),
                        OptionValueSpecification.ForAnyValue("the client.rb file"),
                        OptionValueSpecification.ForExactValue("validator:"),
                        OptionValueSpecification.ForAnyValue("the validator.pem file used to join the node"));
                    var installChefOption = new InstallChefOption(clientFactory.RestClientForChefServer, schedulerWaiter);
                    chefGroup.WithOption(installChefOption, OptionValueSpecification.ForExactValue("install"),
                        OptionValueSpecification.ForVersion());
                    chefGroup.WithOption(installChefOption, OptionValueSpecification.ForExactValue("upgrade"),
                        OptionValueSpecification.ForVersion());
                    chefGroup.WithOption(
                        ChangeChefRunningStatusOption.CreatePauseChefOption(clientFactory.RestClientForChefServer),
                        "pause");
                    chefGroup.WithOption(
                        ChangeChefRunningStatusOption.CreateResumeChefOption(clientFactory.RestClientForChefServer),
                        "resume");
                })
                .WithGroup("server", serverGroup =>
                {
                    serverGroup.WithDefaultOption(new ServerInteractiveOption());
                    serverGroup.WithOption(new ServerWindowsServiceOption(), "--run-as-service");
                })
                .WithGroup("service", serviceGroup =>
                {
                    serviceGroup.WithOption(new RegisterServerWindowsServiceOption(), "register");
                    serviceGroup.WithOption(new UnregisterServerWindowsServiceOption(), "unregister");
                    serviceGroup.WithOption(ChangeStateForCafeWindowsServiceOption.StartCafeWindowsServiceOption(
                        processExecutor, fileSystem,
                        serviceStatusWaiter), "start");
                    serviceGroup.WithOption(ChangeStateForCafeWindowsServiceOption.StopCafeWindowsServiceOption(
                        processExecutor, fileSystem,
                        serviceStatusWaiter), "stop");
                    serviceGroup.WithOption(new CafeWindowsServiceStatusOption(processExecutor, fileSystem),
                        "status");
                })
                .WithGroup("job", statusGroup =>
                {
                    var statusOption = new StatusOption(clientFactory.RestClientForJobServer);
                    statusGroup.WithDefaultOption(statusOption);
                    statusGroup.WithOption(statusOption, "status");
                    statusGroup.WithOption(new JobRunStatusOption(clientFactory.RestClientForJobServer),
                        OptionValueSpecification.ForExactValue("status"), OptionValueSpecification.ForAnyValue("job run id"));
                })
                .WithOption(new InitOption(AssemblyDirectory, environment), "init");
            Logger.Debug("Running application");
            return root;
        }

        public static string LoggingConfigurationFileFor(string[] args)
        {
            return args.FirstOrDefault() == "server" ? ServerLoggingConfigurationFile : ClientLoggingConfigurationFile;
        }
    }
}