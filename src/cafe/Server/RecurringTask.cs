using System;
using NodaTime;

namespace cafe.Server
{
    public class RecurringTask
    {
        private readonly IClock _clock;
        private readonly Duration _every;
        private readonly Func<IScheduledTask> _scheduledTaskCreator;
        private readonly Instant _created;
        private Instant _lastRun;

        public RecurringTask(IClock clock, Duration every, Func<IScheduledTask> scheduledTaskCreator)
        {
            _clock = clock;
            _created = _lastRun = _clock.GetCurrentInstant();
            _every = every;
            _scheduledTaskCreator = scheduledTaskCreator;
        }

        public bool IsReadyToRun => _clock.GetCurrentInstant() >= _lastRun.Plus(_every);

        public IScheduledTask CreateScheduledTask()
        {
            if (!IsReadyToRun)
            {
                throw new InvalidOperationException("Cannot schedule a task that is not yet ready to run");
            }
            _lastRun = _clock.GetCurrentInstant();
            return _scheduledTaskCreator();
        }
    }
}