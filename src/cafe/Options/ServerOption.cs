using System.Collections.Generic;
using cafe.CommandLine;
using System.IO;
using cafe.Chef;
using cafe.Server;
using cafe.Server.Scheduling;
using cafe.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using NodaTime;

namespace cafe.Options
{
    public class ServerOption : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ServerOption).FullName);

        public ServerOption() : base(new OptionSpecification("server"), "Starts cafe in server mode")
        {
        }

        protected override Result RunCore(string[] args)
        {
            var settings = ServerSettings.Read();
            var config = new ConfigurationBuilder()
                // .AddCommandLine(args)
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Build();

            var host = new WebHostBuilder()
                .UseUrls($"http://*:{settings.Port}/")
                .UseConfiguration(config)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            var configurationSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("appsettings.json"));
            Initialize(StructureMapResolver.Container.GetInstance<Scheduler>(), settings.ChefInterval);

            host.Run();
            return Result.Successful();
        }


        public static void Initialize(Scheduler scheduler, int chefIntervalInSeconds)
        {
            if (chefIntervalInSeconds > 0)
            {
                Logger.Info($"Since chef interval duration is set to {chefIntervalInSeconds}, adding chef as a recurring task");
                var interval = Duration.FromSeconds(chefIntervalInSeconds);
                Presenter.ShowMessage($"Scheduling chef to run every {(int)interval.TotalSeconds} seconds", Logger);
                scheduler.Add(new RecurringTask("chef", SystemClock.Instance, interval,
                    () => new ScheduledTask("Run Chef", StructureMapResolver.Container.GetInstance<ChefRunner>().Run, SystemClock.Instance)));
            }
            else
            {
                Logger.Debug($"Since chef interval duration is set to {chefIntervalInSeconds}, not adding chef as a recurring task");
            }
        }

        protected override string ToDescription(string[] args)
        {
            return "Starting Cafe in Server Mode";
        }
    }
}