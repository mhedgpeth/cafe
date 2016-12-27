using System;
using cafe.Server.Scheduling;
using Moq;
using NodaTime;

namespace cafe.Test.Server.Scheduling
{
    public class FakeTimerFactory : ITimerFactory
    {
        private Action _action;
        private Duration _every;

        public IDisposable ExecuteActionOnInterval(Action action, Duration every)
        {
            _action = action;
            _every = every;
            return new Mock<IDisposable>().Object;
        }

        public void FireTimerAction()
        {
            _action();
        }
    }
}