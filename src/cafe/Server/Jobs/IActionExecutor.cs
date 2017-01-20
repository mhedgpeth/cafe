using System;
using System.Threading;

namespace cafe.Server.Scheduling
{
    public interface IActionExecutor
    {
        void Execute(Action action);
    }

    public class RunInBackgroundActionExecutor : IActionExecutor
    {
        public void Execute(Action action)
        {
            ThreadPool.QueueUserWorkItem(state => action());
        }
    }
}