using System;
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
            _scheduledTask = CreateScheduledTask(AssertIsRunning);

            _scheduledTask.Run();

            _actionRan.Should().BeTrue("because the scheduled task ran");
        }

        private ScheduledTask CreateScheduledTask(Action action = null)
        {
            action = action ?? DoNothing;
            return new ScheduledTask("scheduled task", action);
        }

        private void DoNothing()
        {
        }

        [Fact]
        public void Run_ShouldCompleteProperly()
        {
            var scheduledTask = CreateScheduledTask();
            scheduledTask.Run();

            scheduledTask.IsFinishedRunning().Should().BeTrue("because the task has finisehd");
            scheduledTask.IsRunning().Should().BeFalse("because the task is no longer running");
        }

        [Fact]
        public void Id_ShouldBePopulatedAndConsistent()
        {
            var scheduledTask = CreateScheduledTask();

            scheduledTask.Id.Should().Be(scheduledTask.Id);
        }

        [Fact]
        public void ToString_ShouldContainDescription()
        {
            const string description = "a great task";
            var scheduledTask = new ScheduledTask(description, () => { });

            scheduledTask.ToString().Should().Contain(description, "because this was the description given");
        }

        private void AssertIsRunning()
        {
            _scheduledTask.IsRunning().Should().BeTrue("because the task is in the middle of running");
            _scheduledTask.IsFinishedRunning().Should().BeFalse("because the task is not yet finished running");
            _actionRan = true;
        }
    }
}