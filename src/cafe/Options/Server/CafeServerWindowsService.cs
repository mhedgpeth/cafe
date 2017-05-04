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

            Initialize(StructureMapResolver.Container.GetInstance<RunChefJob>(), ServerSettings.Instance.ChefInterval,
                StructureMapResolver.Container.GetInstance<ITimerFactory>(),
                StructureMapResolver.Container.GetInstance<IClock>());

            _webHost.Start();
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