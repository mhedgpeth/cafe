using System;

namespace cafe.CommandLine.LocalSystem
{
    public interface IAutoResetEvent
    {
        void WaitOne(TimeSpan timeout);
        void Set();
    }
}