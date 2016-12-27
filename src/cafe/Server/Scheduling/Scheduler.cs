using System.Collections.Generic;
using cafe.Shared;
using NLog;
using NodaTime;

namespace cafe.Server.Scheduling
{
    public class Scheduler
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Scheduler).FullName);

        private readonly object _processLocker = new object();

        public Scheduler(ITimerFactory timerFactory)
        {
            timerFactory.ExecuteActionOnInterval(ProcessTasks, Duration.FromMinutes(1));
            IsRunning = true;
        }

        public SchedulerStatus CurrentStatus => new SchedulerStatus
        {
            IsRunning = IsRunning,
            QueuedTasks = _queuedTasks.Count
        };

        private readonly Queue<IScheduledTask> _queuedTasks = new Queue<IScheduledTask>();
        private readonly HashSet<RecurringTask> _recurringTasks = new HashSet<RecurringTask>();

        public void ProcessTasks()
        {
            lock (_processLocker) // since queues are being manipulated here, don't let this happen multiple times
            {
                if (!IsRunning)
                {
                    Logger.Debug("Since scheduler is paused, not processing tasks");
                    return;
                }
                AddAllReadyRecurringTasksToQueue();
                if (_queuedTasks.Count == 0)
                {
                    Logger.Debug("There is nothing to do right now");
                    return;
                }
                var readyTask = _queuedTasks.Peek();
                if (readyTask.IsFinishedRunning)
                {
                    Logger.Info($"Task {readyTask} has finished running, so removing it from the queue");
                    _queuedTasks.Dequeue();
                }
                else if (!readyTask.IsRunning)
                {
                    Logger.Info(
                        $"Task {readyTask} is not yet run and it is the next thing to run, so running it");
                    readyTask.Run();
                }
                else
                {
                    Logger.Debug($"Since {readyTask} is still running, waiting for it to complete");
                }
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
                _queuedTasks.Enqueue(task);
            }
        }

        public void Add(RecurringTask recurringTask)
        {
            _recurringTasks.Add(recurringTask);
        }

        public void Pause()
        {
            Logger.Info("Pausing scheduler");
            IsRunning = false;
        }

        public bool IsRunning { get; private set; }
    }
}