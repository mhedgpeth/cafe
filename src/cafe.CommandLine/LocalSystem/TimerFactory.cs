using System;
using System.Threading;
using NodaTime;

namespace cafe.CommandLine.LocalSystem
{
    public class TimerFactory : ITimerFactory
    {
        public IDisposable ExecuteActionOnInterval(Action action, Duration every)
        {
            TimerCallback timerCallback = state => action();
            return new Timer(timerCallback, new object(), every.ToTimeSpan(), every.ToTimeSpan());
        }
    }
}