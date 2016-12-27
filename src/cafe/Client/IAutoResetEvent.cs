using System;

namespace cafe.Client
{
    public interface IAutoResetEvent
    {
        void WaitOne(TimeSpan timeout);
        void Set();
    }
}