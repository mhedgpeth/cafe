using System;

namespace cafe.Server.Scheduling
{
    public interface IScheduledTask
    {
        bool IsFinishedRunning { get; }
        bool IsRunning { get; }
        void Run();
    }

    public class ScheduledTask : IScheduledTask
    {
        private readonly Action _action;

        public ScheduledTask(Action action)
        {
            _action = action;
        }

        public bool IsFinishedRunning { get; private set; }
        public bool IsRunning { get; private set; }
        public void Run()
        {
            IsRunning = true;
            _action();
            IsRunning = false;
            IsFinishedRunning = true;
        }
    }
}