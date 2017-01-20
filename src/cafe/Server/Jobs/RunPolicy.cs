using System;
using cafe.Server.Scheduling;
using NodaTime;

namespace cafe.Server.Jobs
{
    public class RunPolicy
    {
        private readonly Duration? _interval;

        protected RunPolicy(Duration? interval)
        {
            _interval = interval;
        }

        public event EventHandler Due;
        public Duration? Interval => _interval;
        public virtual Instant? ExpectedNextRun { get; } = null;

        protected virtual void OnDue()
        {
            Due?.Invoke(this, EventArgs.Empty);
        }

        public static RunPolicy RegularlyEvery(Duration duration, ITimerFactory timerFactory, IClock clock)
        {
            return new RecurringRunPolicy(duration, timerFactory, clock);
        }

        public static RunPolicy OnDemand()
        {
            return new RunPolicy(null);
        }

        public override string ToString()
        {
            return "On Demand";
        }
    }
}