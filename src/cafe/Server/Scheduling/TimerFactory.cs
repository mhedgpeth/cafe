using System;
using System.Collections.Generic;
using System.Threading;
using NodaTime;

namespace cafe.Server.Scheduling
{
    public class TimerFactory : ITimerFactory
    {
        private readonly IList<Timer> _createdTimers = new List<Timer>();

        public void ExecuteActionOnInterval(Action action, Duration every)
        {
            TimerCallback timerCallback = state => action();
            _createdTimers.Add(new Timer(timerCallback, new object(), every.ToTimeSpan(), every.ToTimeSpan()));
        }

        public void Dispose()
        {
            foreach (var createdTimer in _createdTimers)
            {
                createdTimer.Dispose();
            }
        }
    }
}