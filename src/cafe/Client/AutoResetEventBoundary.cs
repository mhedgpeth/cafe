using System;
using System.Threading;

namespace cafe.Client
{
    public class AutoResetEventBoundary : IAutoResetEvent
    {
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        public void WaitOne(TimeSpan timeout)
        {
            _autoResetEvent.WaitOne(timeout);
        }

        public void Set()
        {
            _autoResetEvent.Set();
        }
    }
}