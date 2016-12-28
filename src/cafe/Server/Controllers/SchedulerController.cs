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

        [HttpGet("recurring/{name}")]
        public RecurringTaskStatus GetRecurringTask(string name)
        {
            Logger.Info($"Getting recurring task named {name}");
            var task = _scheduler.FindRecurringTaskByName(name);
            var status = task?.ToRecurringTaskStatus();
            Logger.Debug($"Status for recurring task {name} is {status}");
            return status;
        }

        [HttpPut("recurring/{name}/pause")]
        public RecurringTaskStatus PauseRecurringTask(string name)
        {
            Logger.Info($"Pausing recurring task {name}");
            var status = _scheduler.PauseRecurringTask(name);
            Logger.Debug($"Finished pausing task {name} with new status of {status}");
            return status;
        }

        [HttpPut("recurring/{name}/resume")]
        public RecurringTaskStatus ResumeRecurringTask(string name)
        {
            Logger.Info($"Resuming recurring task {name}");
            var status = _scheduler.ResumeRecurringTask(name);
            Logger.Debug($"Finished resuming task {name} with new status of {status}");
            return status;
        }
    }
}