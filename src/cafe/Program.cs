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
using cafe.Shared;
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
            var runner = CreateRunner();
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

        private static OptionGroup CreateRunner()
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

            var root = CreateRootGroup(clientFactory, schedulerWaiter, fileSystemCommands, processExecutor, fileSystem, serviceStatusWaiter, environment);
            Logger.Debug("Running application");
            return root;
        }

        public static OptionGroup CreateRootGroup(IClientFactory clientFactory, ISchedulerWaiter schedulerWaiter,
            IFileSystemCommands fileSystemCommands, ProcessExecutor processExecutor, IFileSystem fileSystem,
            ServiceStatusWaiter serviceStatusWaiter, IEnvironment environment)
        {
            var root = new OptionGroup()
                .WithGroup("chef", chefGroup =>
                {
                    const string chefProduct = "Chef";
                    chefGroup.WithOption(new RunChefOption(clientFactory.RestClientForChefServer, schedulerWaiter),
                        "run");
                    chefGroup.WithOption(
                        new DownloadProductOption<IChefServer, ChefStatus>(chefProduct, clientFactory.RestClientForChefServer,
                            schedulerWaiter),
                        CreateDownloadVersionSpecifications());
                    chefGroup.WithOption(new ShowChefStatusOption(clientFactory.RestClientForChefServer), "status");
                    chefGroup.WithOption(
                        new BootstrapChefRunListOption(clientFactory.RestClientForChefServer, schedulerWaiter,
                            fileSystemCommands),
                        OptionValueSpecification.ForCommand("bootstrap"),
                        OptionValueSpecification.ForValue("run-list:", "the run list"),
                        OptionValueSpecification.ForValue("config:", "the client.rb file"),
                        OptionValueSpecification.ForValue("validator:", "the validator.pem file used to join the node"));
                    chefGroup.WithOption(
                        new BootstrapChefPolicyOption(clientFactory.RestClientForChefServer, schedulerWaiter,
                            fileSystemCommands),
                        OptionValueSpecification.ForCommand("bootstrap"),
                        OptionValueSpecification.ForValue("policy:", "the policy name"),
                        OptionValueSpecification.ForValue("group:", "the policy group"),
                        OptionValueSpecification.ForValue("config:", "the client.rb file"),
                        OptionValueSpecification.ForValue("validator:", "the validator.pem file used to join the node"));
                    var installChefOption =
                        new InstallOption<IChefServer, ChefStatus>(chefProduct, clientFactory.RestClientForChefServer,
                            schedulerWaiter);
                    chefGroup.WithOption(installChefOption, CreateInstallVersionSpecifications());
                    chefGroup.WithOption(installChefOption, OptionValueSpecification.ForCommand("upgrade"),
                        OptionValueSpecification.ForVersion());
                    chefGroup.WithOption(
                        ChangeChefRunningStatusOption.CreatePauseChefOption(clientFactory.RestClientForChefServer),
                        "pause");
                    chefGroup.WithOption(
                        ChangeChefRunningStatusOption.CreateResumeChefOption(clientFactory.RestClientForChefServer),
                        "resume");
                })
                .WithGroup("inspec", inspecGroup =>
                {
                    const string inspecProduct = "InSpec";
                    inspecGroup.WithOption(new ShowInSpecStatusOption(clientFactory.RestClientForInspecServer),
                        "status");
                    inspecGroup.WithOption(
                        new InstallOption<IProductServer<ProductStatus>, ProductStatus>(inspecProduct,
                            clientFactory.RestClientForInspecServer, schedulerWaiter), CreateInstallVersionSpecifications());
                    inspecGroup.WithOption(
                        new DownloadProductOption<IProductServer<ProductStatus>, ProductStatus>(inspecProduct,
                            clientFactory.RestClientForInspecServer, schedulerWaiter),
                        CreateDownloadVersionSpecifications());
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
                        OptionValueSpecification.ForCommand("status"),
                        OptionValueSpecification.ForValue("id:", "job run id"));
                })
                .WithOption(new InitOption(AssemblyDirectory, environment), "init");
            return root;
        }

        private static OptionValueSpecification[] CreateDownloadVersionSpecifications()
        {
            return new[]
            {
                OptionValueSpecification.ForCommand("download"),
                OptionValueSpecification.ForVersion()
            };
        }

        private static OptionValueSpecification[] CreateInstallVersionSpecifications()
        {
            return new[]
            {
                OptionValueSpecification.ForCommand("install"),
                OptionValueSpecification.ForVersion()
            };
        }

        public static string LoggingConfigurationFileFor(string[] args)
        {
            return args.FirstOrDefault() == "server" ? ServerLoggingConfigurationFile : ClientLoggingConfigurationFile;
        }
    }
}