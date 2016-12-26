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

        public void ProcessTasks()
        {
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

        public void Schedule(params IScheduledTask[] tasks)
        {
            foreach (var task in tasks)
            {
                _queuedTasks.Add(task);
            }
        }
    }
}