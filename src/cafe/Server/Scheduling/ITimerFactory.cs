using System;
using NodaTime;

namespace cafe.Server.Scheduling
{
    public interface ITimerFactory
    {
        IDisposable ExecuteActionOnInterval(Action action, Duration every);
    }
}