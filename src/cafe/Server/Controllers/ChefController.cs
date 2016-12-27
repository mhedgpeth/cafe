using System;
using System.Threading.Tasks;
using cafe.Chef;
using cafe.Server.Scheduling;
using cafe.Shared;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace cafe.Server.Controllers
{
    [Route("api/[controller]")]
    public class ChefController : Controller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefController).FullName);

        private readonly Scheduler _scheduler = StructureMapResolver.Container.GetInstance<Scheduler>();

        [HttpPut("run")]
        public void RunChef()
        {
            ScheduleAsSoonAsPossible(StructureMapResolver.Container.GetInstance<ChefRunner>().Run);
        }

        private void ScheduleAsSoonAsPossible(Action action)
        {
            _scheduler.Schedule(new ScheduledTask(action));
        }

        [HttpPut("install")]
        [HttpPut("upgrade")]
        public void InstallChef(string version)
        {
            Logger.Info($"Scheduling chef {version} to be installed");
            ScheduleAsSoonAsPossible(() => StructureMapResolver.Container.GetInstance<ChefInstaller>().InstallOrUpgrade(version));
        }

        [HttpPut("download")]
        public void DownloadChef(string version)
        {
            Logger.Info($"Scheduling chef {version} to be downloaded");
            ScheduleAsSoonAsPossible(() => StructureMapResolver.Container.GetInstance<ChefDownloader>().Download(version));
        }

        [HttpGet("status")]
        public ChefStatus GetChefStatus()
        {
            Logger.Info("Getting chef status");
            return new ChefStatus() {Version = StructureMapResolver.Container.GetInstance<ChefRunner>().RetrieveVersion().ToString()};
        }
    }
}