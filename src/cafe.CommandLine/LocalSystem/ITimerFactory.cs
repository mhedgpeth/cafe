using System;
using NodaTime;

namespace cafe.CommandLine.LocalSystem
{
    public interface ITimerFactory
    {
        IDisposable ExecuteActionOnInterval(Action action, Duration every);
    }
}