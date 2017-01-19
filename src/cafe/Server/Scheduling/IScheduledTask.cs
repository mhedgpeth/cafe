using System;
using cafe.Shared;

namespace cafe.Server.Scheduling
{
    public interface IScheduledTask
    {
        Guid Id { get; }
        TaskState CurrentState { get; }
        RecurringTask RecurringTask { get; }
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
}