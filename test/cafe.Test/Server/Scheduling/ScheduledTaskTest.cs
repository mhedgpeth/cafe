using cafe.Server.Scheduling;
using FluentAssertions;
using Xunit;

namespace cafe.Test.Server.Scheduling
{
    public class ScheduledTaskTest
    {
        private ScheduledTask _scheduledTask;
        private bool _actionRan;

        [Fact]
        public void Run_ShouldSetIsRunningToTrue()
        {
            _actionRan = false;
            _scheduledTask = new ScheduledTask(AssertIsRunning);

            _scheduledTask.Run();

            _actionRan.Should().BeTrue("because the scheduled task ran");
        }

        [Fact]
        public void Run_ShouldCompleteProperly()
        {
            var scheduledTask = new ScheduledTask(() => { });
            scheduledTask.Run();

            scheduledTask.IsFinishedRunning.Should().BeTrue("because the task has finisehd");
            scheduledTask.IsRunning.Should().BeFalse("because the task is no longer running");
        }

        private void AssertIsRunning()
        {
            _scheduledTask.IsRunning.Should().BeTrue("because the task is in the middle of running");
            _scheduledTask.IsFinishedRunning.Should().BeFalse("because the task is not yet finished running");
            _actionRan = true;
        }
    }
}