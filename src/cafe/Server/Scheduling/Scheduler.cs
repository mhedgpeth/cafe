using System;
using System.Collections.Generic;
using System.Linq;
using cafe.Shared;
using NLog;
using NLog.Fluent;
using NodaTime;

namespace cafe.Server.Scheduling
{
    public class Scheduler : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Scheduler).FullName);

        private readonly IDisposable _timer;
        private readonly IActionExecutor _scheduledTaskExecutor;
        private readonly object _processLocker = new object();
        private readonly Queue<IScheduledTask> _queuedTasks = new Queue<IScheduledTask>();
        private readonly IDictionary<string, RecurringTask> _recurringTasks = new Dictionary<string, RecurringTask>();
        private readonly IList<ScheduledTaskStatus> _finishedTasks = new List<ScheduledTaskStatus>();

        public Scheduler(ITimerFactory timerFactory, IActionExecutor scheduledTaskExecutor)
        {
            _scheduledTaskExecutor = scheduledTaskExecutor;
            _timer = timerFactory.ExecuteActionOnInterval(ProcessTasks, Duration.FromSeconds(15));
            IsRunning = true;
        }

        public SchedulerStatus CurrentStatus => new SchedulerStatus
        {
            IsRunning = IsRunning,
            QueuedTasks = ConvertQueuedTasksToStatuses(),
            FinishedTasks = _finishedTasks.ToArray(),
            RecurringTasks = ConvertRecurringTasksToStatuses()
        };

        private RecurringTaskStatus[] ConvertRecurringTasksToStatuses()
        {
            return _recurringTasks.Values.Select(r => r.ToRecurringTaskStatus()).ToArray();
        }

        private ScheduledTaskStatus[] ConvertQueuedTasksToStatuses()
        {
            return _queuedTasks.Select(task => task.ToTaskStatus()).ToArray();
        }

        public void ProcessTasks()
        {
            Logger.Debug("Processing tasks for scheduler");
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
            Logger.Debug("Finished processing tasks for scheduler");
        }

        private void AddAllReadyRecurringTasksToQueue()
        {
            foreach (var recurringTask in _recurringTasks.Values)
            {
                if (recurringTask.IsReadyToRun)
                {
                    var scheduledTask = recurringTask.CreateScheduledTask();
                    Log.Info($"Recurring task {recurringTask} is ready to run, so adding {scheduledTask} to the queue");
                    Schedule(scheduledTask);
                }
            }
        }

        public void Schedule(params IScheduledTask[] tasks)
        {
            foreach (var task in tasks)
            {
                Logger.Debug($"Adding scheduled task {task} to the queue of tasks to process");
                _queuedTasks.Enqueue(task);
            }
            ProcessTasks();
        }

        public void Add(RecurringTask recurringTask)
        {
            Logger.Debug($"Adding recurring task {recurringTask} to process");
            _recurringTasks.Add(recurringTask.Name, recurringTask);
        }

        public void Pause()
        {
            Logger.Info("Pausing scheduler");
            IsRunning = false;
        }

        public bool IsRunning { get; private set; }

        public ScheduledTaskStatus FindStatusById(Guid id)
        {
            Logger.Debug(
                $"Searching for task {id} within {_queuedTasks.Count} queued tasks and {_finishedTasks.Count} finished tasks");
            var task = _queuedTasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                var queuedStatus = task.ToTaskStatus();
                Logger.Debug($"Found active status {queuedStatus} for id {id}");
                return queuedStatus;
            }
            var status = _finishedTasks.FirstOrDefault(t => t.Id == id);
            if (status != null)
            {
                Logger.Debug($"Found finished status {status} for id {id}");
            }
            else
            {
                Logger.Debug($"No status for id {id} found");
            }
            return status;
        }

        public RecurringTask FindRecurringTaskByName(string name)
        {
            RecurringTask value;
            _recurringTasks.TryGetValue(name, out value);
            return value;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public RecurringTaskStatus PauseRecurringTask(string name)
        {
            return ExecuteActionOnRecurringTask(name, "Pause", t => t.Pause());
        }

        private RecurringTaskStatus ExecuteActionOnRecurringTask(string name, string description,
            Action<RecurringTask> action)
        {
            Logger.Debug($"Executing action {description} on recurring task {name}");
            RecurringTask task;
            _recurringTasks.TryGetValue(name, out task);
            if (task == null)
            {
                Logger.Warn($"Could not find task named {name} to execute action {description}");
                return null;
            }
            Logger.Debug($"Found recurring task named {name}, executing action {description}");
            action(task);
            var status = task.ToRecurringTaskStatus();
            Logger.Debug($"Action {description} executed; returning status {status}");
            return status;
        }

        public RecurringTaskStatus ResumeRecurringTask(string name)
        {
            return ExecuteActionOnRecurringTask(name, "Resume", t => t.Resume());
        }
    }
}