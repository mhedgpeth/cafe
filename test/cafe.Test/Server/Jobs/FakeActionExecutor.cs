using System;
using cafe.Server.Jobs;

namespace cafe.Test.Server.Jobs
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