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
            CurrentState = FinishTaskImmediately ? JobRunState.Finished : JobRunState.Running;
        }

        public bool IsFinishedRunning => CurrentState == JobRunState.Finished;
        public bool IsRunning => CurrentState == JobRunState.Running;

        public JobRunState CurrentState { get; set; }
        public Guid Id { get; } = Guid.NewGuid();

        public JobRunStatus ToStatus(int? previousIndex = null)
        {
            return new JobRunStatus() { Id = Id};
        }

        public JobRunStatus ToTaskStatus()
        {
            return new JobRunStatus()
            {
                Id = Id,
                Description = "fake task",
                State = CurrentState,
                FinishTime = Ended?.ToDateTimeUtc()
            };
        }

        public Instant? Started { get; set; }
        public Instant? Ended { get; set; }

        public void FinishTask(Instant endTime)
        {
            CurrentState = JobRunState.Finished;
            Ended = endTime;
        }
    }
}