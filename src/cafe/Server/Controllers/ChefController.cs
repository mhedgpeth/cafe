using System;
using cafe.Chef;
using cafe.LocalSystem;
using cafe.Server.Scheduling;
using Microsoft.AspNetCore.Mvc;

namespace cafe.Server.Controllers
{
    [Route("api/[controller]")]
    public class ChefController : Controller
    {
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
            ScheduleAsSoonAsPossible(() => StructureMapResolver.Container.GetInstance<ChefInstaller>().InstallOrUpgrade(version));
        }

        [HttpPut("download")]
        public void DownloadChef(string version)
        {
            ScheduleAsSoonAsPossible(() => StructureMapResolver.Container.GetInstance<ChefDownloader>().Download(version));
        }

        [HttpGet("status")]
        public ChefStatus GetChefStatus()
        {
            return new ChefStatus() {Version = StructureMapResolver.Container.GetInstance<ChefRunner>().RetrieveVersion().ToString()};
        }
    }

    public class ChefStatus
    {
        public string Version { get; set; }
    }
}