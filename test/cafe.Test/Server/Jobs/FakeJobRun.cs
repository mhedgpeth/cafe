using System;
using cafe.Server.Jobs;
using cafe.Shared;
using NodaTime;

namespace cafe.Test.Server.Jobs
{
    public class FakeJobRun : IJobRun
    {
        public bool WasRunCalled { get; set; }

        public bool FinishTaskImmediately { get; set; } = true;

        public void Run()
        {
            WasRunCalled = true;
            CurrentState = FinishTaskImmediately ? TaskState.Finished : TaskState.Running;
        }

        public bool IsFinishedRunning => CurrentState == TaskState.Finished;
        public bool IsRunning => CurrentState == TaskState.Running;

        public TaskState CurrentState { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
        public ScheduledTaskStatus ToStatus()
        {
            return new ScheduledTaskStatus() { Id = Id};
        }

        public ScheduledTaskStatus ToTaskStatus()
        {
            return new ScheduledTaskStatus()
            {
                Id = Id,
                Description = "fake task",
                State = CurrentState,
                CompleteTime = Ended?.ToDateTimeUtc()
            };
        }

        public Instant? Started { get; set; }
        public Instant? Ended { get; set; }

        public void FinishTask(Instant endTime)
        {
            CurrentState = TaskState.Finished;
            Ended = endTime;
        }
    }
}