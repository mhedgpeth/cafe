using System;
using cafe.Server.Scheduling;
using cafe.Shared;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace cafe.Server.Controllers
{
    [Route("api/[controller]")]
    public class SchedulerController : Controller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(SchedulerController).FullName);

        private readonly Scheduler _scheduler = StructureMapResolver.Container.GetInstance<Scheduler>();

        [HttpGet("status")]
        public SchedulerStatus GetStatus()
        {
            Logger.Info("Getting scheduler status");
            var schedulerStatus = _scheduler.CurrentStatus;
            Logger.Debug($"Scheduler status is {schedulerStatus}");
            return schedulerStatus;
        }

        [HttpGet("task/{id}")]
        public ScheduledTaskStatus GetTaskStatus(Guid id)
        {
            Logger.Info($"Getting status of task with id {id}");
            var status = _scheduler.FindStatusById(id);
            Logger.Debug($"Status for task {id} is {status}");
            return status;
        }

        [HttpPut("pause")]
        public void Pause()
        {
            Logger.Info("Pausing scheduler");
            _scheduler.Pause();
        }
    }
}