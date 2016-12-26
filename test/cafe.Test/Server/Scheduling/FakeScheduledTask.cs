using cafe.Server.Scheduling;

namespace cafe.Test.Server.Scheduling
{
    public class FakeScheduledTask : IScheduledTask
    {
        public bool IsFinishedRunning { get; set; }
        public bool WasRunCalled { get; set; }
        public bool IsRunning { get; set; }

        public bool FinishTaskImmediately { get; set; } = true;

        public void Run()
        {
            WasRunCalled = true;
            IsFinishedRunning = FinishTaskImmediately;
            IsRunning = !IsFinishedRunning;
        }
    }
}