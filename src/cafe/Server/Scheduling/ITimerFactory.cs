using System;
using NodaTime;

namespace cafe.Server.Scheduling
{
    public interface ITimerFactory : IDisposable
    {
        void ExecuteActionOnInterval(Action action, Duration every);
    }
}