using System.IO;
using cafe.CommandLine;
using cafe.CommandLine.LocalSystem;
using cafe.Server;
using cafe.Server.Jobs;
using DasMulli.Win32.ServiceUtils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NodaTime;

namespace cafe.Options.Server
{
    public class CafeServerWindowsService : IWin32Service
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(CafeServerWindowsService).FullName);
        private IWebHost _webHost;
        private bool _stopRequestedByWindows;

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            Logger.Info("Starting service");
            var config = new ConfigurationBuilder()
                // .AddCommandLine(args)
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Build();

            _webHost = new WebHostBuilder()
                .UseUrls($"http://*:{ServerSettings.Instance.Port}/")
                .UseConfiguration(config)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            _webHost.Services.GetRequiredService<IApplicationLifetime>()
                .ApplicationStopped
                .Register(() =>
                {
                    if (!_stopRequestedByWindows)
                    {
                        serviceStoppedCallback();
                    }
                });

            Initialize();

            ReactToChangesToServerConfiguration();
            ReactToChangesToChefClientRunning();
            
            _webHost.Start();
        }

        private static void ReactToChangesToServerConfiguration()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(".") {Filter = "server.json"};
            watcher.Changed += OnServerConfigurationChanged;
            watcher.EnableRaisingEvents = true;
        }

        private static void ReactToChangesToChefClientRunning()
        {
            var clientRunningFile = "chef-client-running.pid";
            var clientRunningFolder = $"{ServerSettings.Instance.InstallRoot}/chef/cache";
            Logger.Info($"Listening for chef client to be running by listening for pid file {clientRunningFile} in {clientRunningFolder}");
            FileSystemWatcher watcher = new FileSystemWatcher(clientRunningFolder) { Filter = clientRunningFile };
            watcher.Created += PauseRunner;
            watcher.Deleted += ResumeRunner;
        }

        private static void ResumeRunner(object sender, FileSystemEventArgs e)
        {
            var runner = StructureMapResolver.Container.GetInstance<JobRunner>();
            Logger.Info($"Since file {e.FullPath} has been deleted and therefore the chef client has stopped running, resuming processing of jobs");
            runner.Pause();
        }

        private static void PauseRunner(object sender, FileSystemEventArgs e)
        {
            var runner = StructureMapResolver.Container.GetInstance<JobRunner>();
            Logger.Info($"Since file {e.FullPath} has been created and therefore the chef client is running, pausing any processing of jobs so we don't interfere with Chef's activities");
            runner.Pause();
        }

        private static void OnServerConfigurationChanged(object sender, FileSystemEventArgs args)
        {
            Presenter.ShowMessage("Server configuration changed, so resetting Chef Interval", Logger);
            ServerSettings.Reload();
            Initialize();
        }

        private static void Initialize()
        {
            Initialize(StructureMapResolver.Container.GetInstance<RunChefJob>(), ServerSettings.Instance.ChefInterval,
                StructureMapResolver.Container.GetInstance<ITimerFactory>(),
                StructureMapResolver.Container.GetInstance<IClock>());
        }

        public static void Initialize(RunChefJob runChefJob, int chefIntervalInSeconds, ITimerFactory timerFactory,
            IClock clock)
        {
            if (chefIntervalInSeconds > 0)
            {
                var interval = Duration.FromSeconds(chefIntervalInSeconds);
                Presenter.ShowMessage($"Scheduling chef to run every {(int) interval.TotalSeconds} seconds", Logger);
                runChefJob.RunPolicy = RunPolicy.RegularlyEvery(interval, timerFactory, clock);
            }
            else
            {
                Logger.Debug(
                    $"Since chef interval duration is set to {chefIntervalInSeconds}, not adding chef as a recurring task");
            }
        }


        public void Stop()
        {
            Logger.Info("Stopping service");
            _stopRequestedByWindows = true;
            _webHost.Dispose();
        }

        public string ServiceName => "cafe";
    }
}