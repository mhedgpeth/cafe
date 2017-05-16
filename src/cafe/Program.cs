using System;
using System.IO;
using System.Linq;
using System.Reflection;
using cafe.Client;
using cafe.CommandLine;
using cafe.CommandLine.LocalSystem;
using cafe.CommandLine.Options;
using cafe.LocalSystem;
using cafe.Options;
using cafe.Options.Chef;
using cafe.Options.Server;
using cafe.Shared;
using DasMulli.Win32.ServiceUtils;
using NLog;
using NLog.Config;

namespace cafe
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Program).FullName);

        public static int Main(string[] args)
        {
            Directory.SetCurrentDirectory(AssemblyDirectory);
            LoggingInitializer.ConfigureLogging(args);
            Presenter.ShowApplicationHeading(Logger, Assembly.GetEntryAssembly().GetName().Version, args);
            var clientFactory = CreateClientFactory();
            var runner = CreateRunner(clientFactory);
            var arguments = runner.ParseArguments(args);
            if (arguments != null)
            {
                if (arguments.HasArgumentLabeled("on:"))
                {
                    var hostname = arguments.FindValueFromLabel("on:").Value;
                    Presenter.ShowMessage($"on node: {hostname}", Logger);
                    clientFactory.Hostname = hostname;
                }
                var returnValue = runner.RunProgram(arguments);
                Logger.Debug("Finishing cafe run");
                return returnValue;
            }
            else
            {
                Presenter.ShowError("No options match the supplied arguments. Run cafe help to view all options",
                    Logger);
                return -2;
            }
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

        private static OptionGroup CreateRunner(IClientFactory clientFactory)
        {
            var schedulerWaiter = new SchedulerWaiter(clientFactory.RestClientForJobServer,
                new AutoResetEventBoundary(), new TimerFactory(),
                new PresenterMessagePresenter());
            var processExecutor = new ProcessExecutor(() => new ProcessBoundary());
            var environment = new EnvironmentBoundary();
            var fileSystemCommands = new FileSystemCommandsBoundary();
            var fileSystem = new FileSystem(environment, fileSystemCommands);
            var serviceStatusWaiter = new ServiceStatusWaiter("waiting for service status",
                new AutoResetEventBoundary(), new TimerFactory(),
                new ServiceStatusProvider(processExecutor, fileSystem), CafeServerWindowsServiceOptions.ServiceName);
            // all options available

            var root = CreateRootGroup(clientFactory, schedulerWaiter, fileSystemCommands, processExecutor, fileSystem,
                serviceStatusWaiter, environment);
            Logger.Debug("Running application");
            return root;
        }

        private static ClientFactory CreateClientFactory()
        {
            var clientFactory = new ClientFactory(ClientSettings.Instance.Node, ClientSettings.Instance.Port);
            return clientFactory;
        }

        public static OptionGroup CreateRootGroup(IClientFactory clientFactory, ISchedulerWaiter schedulerWaiter,
            IFileSystemCommands fileSystemCommands, ProcessExecutor processExecutor, IFileSystem fileSystem,
            ServiceStatusWaiter serviceStatusWaiter, IEnvironment environment)
        {
            var root = new OptionGroup()
                .WithGroup("chef", chefGroup =>
                {
                    const string chefProduct = "Chef";
                    Func<IChefServer> restClientForChefServerFactory = clientFactory.RestClientForChefServer;
                    chefGroup.WithOption(new RunChefOption(restClientForChefServerFactory, schedulerWaiter),
                        OptionValueSpecification.ForCommand("run"), OnNode());
                    chefGroup.WithOption(
                        new DownloadProductOption<IChefServer, ChefStatus>(chefProduct,
                            restClientForChefServerFactory,
                            schedulerWaiter),
                        CreateDownloadVersionSpecifications());
                    chefGroup.WithOption(new ShowChefStatusOption(restClientForChefServerFactory),
                        OptionValueSpecification.ForCommand("status"), OnNode());
                    chefGroup.WithOption(
                        new BootstrapChefRunListOption(restClientForChefServerFactory, schedulerWaiter,
                            fileSystemCommands),
                        OptionValueSpecification.ForCommand("bootstrap"),
                        OptionValueSpecification.ForValue("run-list:", "the run list"),
                        OptionValueSpecification.ForValue("config:", "the client.rb file"),
                        OptionValueSpecification.ForValue("validator:", "the validator.pem file used to join the node"),
                        OnNode());
                    chefGroup.WithOption(
                        new BootstrapChefPolicyOption(restClientForChefServerFactory, schedulerWaiter,
                            fileSystemCommands),
                        OptionValueSpecification.ForCommand("bootstrap"),
                        OptionValueSpecification.ForValue("policy:", "the policy name"),
                        OptionValueSpecification.ForValue("group:", "the policy group"),
                        OptionValueSpecification.ForValue("config:", "the client.rb file"),
                        OptionValueSpecification.ForValue("validator:", "the validator.pem file used to join the node"),
                        OnNode());
                    var installChefOption =
                        new InstallOption<IChefServer, ChefStatus>(chefProduct, restClientForChefServerFactory,
                            schedulerWaiter);
                    chefGroup.WithOption(installChefOption, CreateInstallVersionSpecifications());
                    chefGroup.WithOption(installChefOption, OptionValueSpecification.ForCommand("upgrade"),
                        OptionValueSpecification.ForVersion(), OnNode(), ReturnImmediatelyOrDelayed());
                    chefGroup.WithOption(
                        ChangeChefRunningStatusOption.CreatePauseChefOption(restClientForChefServerFactory),
                        OptionValueSpecification.ForCommand("pause"), OnNode());
                    chefGroup.WithOption(
                        ChangeChefRunningStatusOption.CreateResumeChefOption(restClientForChefServerFactory),
                        OptionValueSpecification.ForCommand("resume"), OnNode());
                    chefGroup.WithOption(
                        new CheckProductVersionOption("chef", clientFactory.GenericRestClientForChefServer),
                        OptionValueSpecification.ForCommand("version?"),
                        OptionValueSpecification.ForVersion(), OnNode());
                })
                .WithGroup("inspec", inspecGroup =>
                {
                    const string inspecProduct = "inspec";
                    Func<IProductServer<ProductStatus>> productServerFactory = clientFactory.RestClientForInspecServer;
                    AddProductOptionsTo(inspecGroup, inspecProduct, productServerFactory, schedulerWaiter);
                })
                .WithGroup("server", serverGroup =>
                {
                    const string application = "cafe";
                    Func<IWin32Service> serviceCreator = () => new CafeServerWindowsService();
                    serverGroup.WithDefaultOption(new ServerInteractiveOption(application, serviceCreator));
                    serverGroup.WithOption(new ServerWindowsServiceOption(application, serviceCreator),
                        "--run-as-service");
                })
                .WithGroup("service",
                    serviceGroup =>
                    {
                        ServiceOptionInitializer.AddServiceOptionsTo(serviceGroup, serviceStatusWaiter, processExecutor,
                            fileSystem, CafeServerWindowsServiceOptions.ServiceName,
                            CafeServerWindowsServiceOptions.ServiceDisplayName,
                            CafeServerWindowsServiceOptions.ServiceDescription);
                    })
                .WithGroup("job", statusGroup =>
                {
                    var statusOption = new StatusOption(clientFactory.RestClientForJobServer);
                    statusGroup.WithDefaultOption(statusOption);
                    statusGroup.WithOption(statusOption, OptionValueSpecification.ForCommand("status"),
                        OnNode());
                    statusGroup.WithOption(new JobRunStatusOption(clientFactory.RestClientForJobServer),
                        OptionValueSpecification.ForCommand("status"),
                        OptionValueSpecification.ForValue("id:", "job run id"),
                        OnNode());
                })
                .WithOption(new InitOption(AssemblyDirectory, environment), "init");
            AddProductOptionsTo(root, "cafe", clientFactory.RestClientForCafeProductServer, schedulerWaiter);

            var helpOption = new HelpOption(root);
            root.WithOption(helpOption, "help");
            root.WithDefaultOption(helpOption);

            return root;
        }


        private static void AddProductOptionsTo(OptionGroup productGroup, string productName,
            Func<IProductServer<ProductStatus>> productServerFactory, ISchedulerWaiter schedulerWaiter)
        {
            productGroup.WithOption(new ShowInSpecStatusOption(productServerFactory),
                OptionValueSpecification.ForCommand("status"), OnNode());
            productGroup.WithOption(
                new InstallOption<IProductServer<ProductStatus>, ProductStatus>(productName,
                    productServerFactory, schedulerWaiter),
                CreateInstallVersionSpecifications());
            productGroup.WithOption(
                new DownloadProductOption<IProductServer<ProductStatus>, ProductStatus>(productName,
                    productServerFactory, schedulerWaiter),
                CreateDownloadVersionSpecifications());
            AddCheckProductVersionTo(productGroup, productName, productServerFactory);
        }

        private static void AddCheckProductVersionTo(OptionGroup inspecGroup, string productName,
            Func<IProductServer<ProductStatus>> restClient)
        {
            inspecGroup.WithOption(
                new CheckProductVersionOption(productName, restClient),
                OptionValueSpecification.ForCommand("version?"),
                OptionValueSpecification.ForVersion(), OnNode());
        }


        private static OptionValueSpecification ReturnImmediatelyOrDelayed()
        {
            return OptionValueSpecification.ForOptionalValue("return:", "immediately or afterFinished (default)");
        }

        private static OptionValueSpecification OnNode()
        {
            return OptionValueSpecification.ForOptionalValue("on:", "node");
        }

        private static OptionValueSpecification[] CreateDownloadVersionSpecifications()
        {
            return new[]
            {
                OptionValueSpecification.ForCommand("download"),
                OptionValueSpecification.ForVersion(),
                OnNode()
            };
        }

        private static OptionValueSpecification[] CreateInstallVersionSpecifications()
        {
            return new[]
            {
                OptionValueSpecification.ForCommand("install"),
                OptionValueSpecification.ForVersion(),
                OnNode(),
                ReturnImmediatelyOrDelayed()
            };
        }
    }
}