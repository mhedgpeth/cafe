using System;
using cafe.Server.Scheduling;

namespace cafe.Test.Server.Scheduling
{
    public class FakeActionExecutor : IActionExecutor
    {
        public void Execute(Action action)
        {
            WasExecuted = true;
            action();
        }

        public bool WasExecuted { get; set; }
    }
}