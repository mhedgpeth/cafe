using System;
using System.Threading.Tasks;
using cafe.Chef;
using cafe.Server.Scheduling;
using cafe.Shared;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NodaTime;

namespace cafe.Server.Controllers
{
    [Route("api/[controller]")]
    public class ChefController : Controller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefController).FullName);

        private readonly Scheduler _scheduler = StructureMapResolver.Container.GetInstance<Scheduler>();

        [HttpPut("run")]
        public ScheduledTaskStatus RunChef()
        {
            return ScheduleAsSoonAsPossible("Run Chef", StructureMapResolver.Container.GetInstance<ChefRunner>().Run);
        }

        private ScheduledTaskStatus ScheduleAsSoonAsPossible(string description, Func<Result> action)
        {
            var scheduledTask = new ScheduledTask(description, action, SystemClock.Instance);
            _scheduler.Schedule(scheduledTask);
            return scheduledTask.ToTaskStatus();
        }

        [HttpPut("install")]
        [HttpPut("upgrade")]
        public ScheduledTaskStatus InstallChef(string version)
        {
            Logger.Info($"Scheduling chef {version} to be installed");
            return ScheduleAsSoonAsPossible($"Install/Upgrade Chef to {version}", () => StructureMapResolver.Container.GetInstance<ChefInstaller>().InstallOrUpgrade(version));
        }

        [HttpPut("download")]
        public ScheduledTaskStatus DownloadChef(string version)
        {
            Logger.Info($"Scheduling chef {version} to be downloaded");
            return ScheduleAsSoonAsPossible($"Download Chef {version}", () => StructureMapResolver.Container.GetInstance<ChefDownloader>().Download(version));
        }

        [HttpGet("status")]
        public ChefStatus GetChefStatus()
        {
            Logger.Info("Getting chef status");
            return new ChefStatus() {Version = StructureMapResolver.Container.GetInstance<ChefRunner>().RetrieveVersion().ToString()};
        }
    }
}