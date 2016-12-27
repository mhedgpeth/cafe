using System;
using System.Collections.Generic;
using System.Linq;
using cafe.Shared;
using NLog;
using NodaTime;

namespace cafe.Server.Scheduling
{
    public class Scheduler : IDisposable
    {
        private readonly Guid _instanceId = Guid.NewGuid();
        private readonly ITimerFactory _timerFactory;
        private readonly IActionExecutor _scheduledTaskExecutor;
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Scheduler).FullName);

        private readonly object _processLocker = new object();

        public Scheduler(ITimerFactory timerFactory, IActionExecutor scheduledTaskExecutor)
        {
            _timerFactory = timerFactory;
            _scheduledTaskExecutor = scheduledTaskExecutor;
            _timerFactory.ExecuteActionOnInterval(ProcessTasks, Duration.FromSeconds(15));
            IsRunning = true;
            Console.Out.WriteLine(_instanceId);
        }

        public SchedulerStatus CurrentStatus => new SchedulerStatus
        {
            IsRunning = IsRunning,
            QueuedTasks = ConvertQueuedTasksToStatuses(),
            FinishedTasks = _finishedTasks.ToArray()
        };

        private ScheduledTaskStatus[] ConvertQueuedTasksToStatuses()
        {
            return _queuedTasks.Select(task => task.ToTaskStatus()).ToArray();
        }

        private readonly Queue<IScheduledTask> _queuedTasks = new Queue<IScheduledTask>();
        private readonly HashSet<RecurringTask> _recurringTasks = new HashSet<RecurringTask>();
        private readonly IList<ScheduledTaskStatus> _finishedTasks = new List<ScheduledTaskStatus>();

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
                if (readyTask.IsFinishedRunning())
                {
                    Logger.Info($"Task {readyTask} has finished running, so removing it from the queue");
                    _queuedTasks.Dequeue();
                    _finishedTasks.Add(readyTask.ToTaskStatus());
                }
                else if (!readyTask.IsRunning())
                {
                    Logger.Info(
                        $"Task {readyTask} is not yet run and it is the next thing to run, so running it");
                    _scheduledTaskExecutor.Execute(readyTask.Run);
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

        public ScheduledTaskStatus FindStatusById(Guid id)
        {
            var task = _queuedTasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                return task.ToTaskStatus();
            }
            return _finishedTasks.FirstOrDefault(t => t.Id == id);
        }

        public void Dispose()
        {
            _timerFactory.Dispose();
        }
    }
}