using System;
using cafe.Shared;

namespace cafe.Server.Scheduling
{
    public interface IScheduledTask
    {
        Guid Id { get; }
        TaskState CurrentState { get; }
        void Run();
        ScheduledTaskStatus ToTaskStatus();
    }

    public static class ScheduledTaskExtensions
    {
        public static bool IsFinishedRunning(this IScheduledTask task)
        {
            return task.CurrentState == TaskState.Finished;
        }

        public static bool IsRunning(this IScheduledTask task)
        {
            return task.CurrentState == TaskState.Running;
        }
    }


    public class ScheduledTask : IScheduledTask
    {
        private readonly ScheduledTaskStatus _status;
        private readonly Action _action;

        public ScheduledTask(string description, Action action)
        {
            _action = action;
            _status = ScheduledTaskStatus.Create(description);
        }

        public void Run()
        {
            _status.State = TaskState.Running;
            _action();
            _status.State = TaskState.Finished;
        }

        public TaskState CurrentState { get { return _status.State; } }
        public Guid Id { get; } = Guid.NewGuid();

        public override string ToString()
        {
            return _status.ToString();
        }

        public ScheduledTaskStatus ToTaskStatus()
        {
            return _status.Copy();
        }
    }
}