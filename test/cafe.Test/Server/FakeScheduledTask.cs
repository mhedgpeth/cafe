using cafe.Server;

namespace cafe.Test.Server
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