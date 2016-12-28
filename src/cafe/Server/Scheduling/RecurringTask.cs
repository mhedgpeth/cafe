using System;
using cafe.Shared;
using NodaTime;

namespace cafe.Server.Scheduling
{
    public class RecurringTask
    {
        private readonly IClock _clock;
        private readonly Duration _interval;
        private readonly Func<IScheduledTask> _scheduledTaskCreator;
        private readonly Instant _created;
        private readonly string _name;
        private Instant? _lastRun;

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

        public bool IsReadyToRun => _clock.GetCurrentInstant() >= ExpectedNextRun;

        private Instant ExpectedNextRun => LastCheckpoint.Plus(_interval);

        public Duration Interval => _interval;

        public IScheduledTask CreateScheduledTask()
        {
            if (!IsReadyToRun)
            {
                throw new InvalidOperationException("Cannot schedule a task that is not yet ready to run");
            }
            _lastRun = _clock.GetCurrentInstant();
            return _scheduledTaskCreator();
        }

        public RecurringTaskStatus ToRecurringTaskStatus()
        {
            return new RecurringTaskStatus()
            {
                Name = _name,
                Created = _created.ToDateTimeUtc(),
                Interval = _interval.ToTimeSpan(),
                LastRun = _lastRun?.ToDateTimeUtc(),
                ExpectedNextRun = ExpectedNextRun.ToDateTimeUtc()
            };
        }
    }
}