using System;
using cafe.Shared;

namespace cafe.Server.Jobs
{
    public interface IJobRun
    {
        void Run();
        bool IsFinishedRunning { get; }
        bool IsRunning { get; }
        Guid Id { get; }
        ScheduledTaskStatus ToStatus();
    }
}