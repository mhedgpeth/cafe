using System;
using cafe.Shared;
using NLog;
using NodaTime;

namespace cafe.Server.Scheduling
{
    public class RecurringTask
    {
        public const string RunChefKey = "chef";

        private static readonly Logger Logger = LogManager.GetLogger(typeof(RecurringTask).FullName);

        private readonly IClock _clock;
        private readonly Duration _interval;
        private readonly Func<IScheduledTask> _scheduledTaskCreator;
        private readonly Instant _created;
        private readonly string _name;
        private Instant? _lastRun;
        private IScheduledTask _nextScheduledTask;

        public RecurringTask(string name, IClock clock, Duration interval, Func<IScheduledTask> scheduledTaskCreator)
        {
            _clock = clock;
            _created = _clock.GetCurrentInstant();
            _interval = interval;
            _scheduledTaskCreator = scheduledTaskCreator;
            _name = name;
        }

        public string Name => _name;

        private Instant LastCheckpoint => _lastRun ?? _created;

        public bool IsReadyToRun => _nextScheduledTask != null || (IsRunning && _clock.GetCurrentInstant() >= ExpectedNextRun);

        private Instant ExpectedNextRun => LastCheckpoint.Plus(_interval);

        public Duration Interval => _interval;

        public IScheduledTask ProvideNextScheduledTask()
        {
            if (!IsReadyToRun)
            {
                throw new InvalidOperationException("Cannot schedule a task that is not yet ready to run");
            }
            _lastRun = _clock.GetCurrentInstant();
            var next = _nextScheduledTask ?? _scheduledTaskCreator();
            Logger.Debug($"Providing {next} as the next scheduled task with immediate task: {_nextScheduledTask}");
            _nextScheduledTask = null;
            return next;
        }

        public RecurringTaskStatus ToRecurringTaskStatus()
        {
            return new RecurringTaskStatus()
            {
                Name = _name,
                IsRunning = IsRunning,
                Created = _created.ToDateTimeUtc(),
                Interval = _interval.ToTimeSpan(),
                LastRun = _lastRun?.ToDateTimeUtc(),
                ExpectedNextRun = ExpectedNextRun.ToDateTimeUtc()
            };
        }

        public bool IsRunning { get; private set; } = true;

        public void Pause()
        {
            Logger.Info($"Pausing {Name}");
            IsRunning = false;
        }

        public void Resume()
        {
            Logger.Info($"Resuming {Name}");
            IsRunning = true;
        }

        public event EventHandler ScheduledTaskReady;

        public void RunTaskImmediately(IScheduledTask scheduledTask)
        {
            Logger.Debug($"Providing {scheduledTask} the next time it's asked for");
            _nextScheduledTask = scheduledTask;
            OnScheduledTaskReady();
        }

        protected virtual void OnScheduledTaskReady()
        {
            ScheduledTaskReady?.Invoke(this, EventArgs.Empty);
        }
    }
}