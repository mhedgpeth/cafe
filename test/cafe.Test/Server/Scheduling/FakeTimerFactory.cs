using System;
using cafe.Server.Scheduling;
using NodaTime;

namespace cafe.Test.Server.Scheduling
{
    public class FakeTimerFactory : ITimerFactory
    {
        private Action _action;
        private Duration _every;

        public void Dispose()
        {
        }

        public void ExecuteActionOnInterval(Action action, Duration every)
        {
            _action = action;
            _every = every;
        }

        public void FireTimerAction()
        {
            _action();
        }
    }
}