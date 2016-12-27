using System;
using cafe.Server.Scheduling;
using cafe.Shared;

namespace cafe.Test.Server.Scheduling
{
    public class FakeScheduledTask : IScheduledTask
    {
        public bool WasRunCalled { get; set; }

        public bool FinishTaskImmediately { get; set; } = true;

        public void Run()
        {
            WasRunCalled = true;
            CurrentState = FinishTaskImmediately ? TaskState.Finished : TaskState.Running;
        }

        public TaskState CurrentState { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
        public ScheduledTaskStatus ToTaskStatus()
        {
            return new ScheduledTaskStatus()
            {
                Id = Id,
                Description = "fake task",
                State = CurrentState
            };
        }
    }
}