using System;
using cafe.Server.Scheduling;
using NodaTime;

namespace cafe.Server.Jobs
{
    public class RunPolicy
    {
        protected RunPolicy()
        {
        }

        public event EventHandler Due;

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
            return new RunPolicy();
        }

        public override string ToString()
        {
            return "On Demand";
        }
    }
}