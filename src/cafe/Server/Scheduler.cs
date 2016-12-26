using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace cafe.Server
{
    public class Scheduler
    {
        private static ILogger Logger { get; } =
          ApplicationLogging.CreateLogger<Scheduler>();

        private readonly HashSet<IScheduledTask> _queuedTasks = new HashSet<IScheduledTask>();
        private readonly HashSet<RecurringTask> _recurringTasks = new HashSet<RecurringTask>();
        public void ProcessTasks()
        {
            AddAllReadyRecurringTasksToQueue();
            var readyTask = _queuedTasks.FirstOrDefault();
            if (readyTask == null)
            {
                Logger.LogDebug("There is nothing to do right now");
            }
            else if (readyTask.IsFinishedRunning)
            {
                Logger.LogInformation($"Task {readyTask} has finished running, so removing it from the queue");
                _queuedTasks.Remove(readyTask);
            }
            else if (!readyTask.IsRunning)
            {
                Logger.LogInformation($"Task {readyTask} is not yet run and it is the next thing to run, so running it");
                readyTask.Run();
            }
            else
            {
                Logger.LogDebug($"Since {readyTask} is still running, waiting for it to complete");
            }
        }

        private void AddAllReadyRecurringTasksToQueue()
        {
            foreach (var recurringTask in _recurringTasks)
            {
                if (recurringTask.IsReadyToRun)
                {
                    Schedule(recurringTask.CreateScheduledTask());
                }
            }
        }

        public void Schedule(params IScheduledTask[] tasks)
        {
            foreach (var task in tasks)
            {
                _queuedTasks.Add(task);
            }
        }

        public void Add(RecurringTask recurringTask)
        {
            _recurringTasks.Add(recurringTask);
        }
    }
}