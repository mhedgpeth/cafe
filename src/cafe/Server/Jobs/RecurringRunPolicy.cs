
using System;
using cafe.Server.Scheduling;
using NLog;
using NodaTime;

namespace cafe.Server.Jobs
{
    public class RecurringRunPolicy : RunPolicy, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(RecurringRunPolicy).FullName);

        private readonly Duration _every;
        private readonly IClock _clock;
        private readonly IDisposable _timer;
        private Instant _lastFired;

        public RecurringRunPolicy(Duration every, ITimerFactory timerFactory, IClock clock)
        {
            _every = every;
            _clock = clock;
            _timer = timerFactory.ExecuteActionOnInterval(ProcessTimerFired, every);
            Logger.Info($"Creating recurring run every {every}");
            _lastFired = clock.GetCurrentInstant();
        }

        private void ProcessTimerFired()
        {
            Logger.Debug("Timer fired, firing that recurring run is due");
            _lastFired = _clock.GetCurrentInstant();
            OnDue();
        }

        public Duration Every => _every;
        public Instant ExpectedNextRun => _lastFired.Plus(Every);

        public void Dispose()
        {
            _timer.Dispose();
        }

        public override string ToString()
        {
            return $"Every {_every.Seconds} seconds";
        }
    }
}