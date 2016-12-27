using System;
using cafe.Server.Scheduling;
using cafe.Shared;
using FluentAssertions;
using NodaTime;
using Xunit;

namespace cafe.Test.Server.Scheduling
{
    public class ScheduledTaskTest
    {
        private ScheduledTask _scheduledTask;
        private bool _actionRan;
        private FakeClock _clock;

        [Fact]
        public void Run_ShouldSetIsRunningToTrue()
        {
            _actionRan = false;
            _scheduledTask = CreateScheduledTask(AssertIsRunning);

            _scheduledTask.Run();

            _actionRan.Should().BeTrue("because the scheduled task ran");
        }

        private ScheduledTask CreateScheduledTask(Func<Result> action = null, IClock clock = null)
        {
            action = action ?? DoNothing;
            clock = clock ?? new FakeClock();
            return new ScheduledTask("scheduled task", action, clock);
        }

        private Result DoNothing()
        {
            return Result.Successful();
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
            var scheduledTask = new ScheduledTask(description, DoNothing, new FakeClock());

            scheduledTask.ToString().Should().Contain(description, "because this was the description given");
        }

        private Result AssertIsRunning()
        {
            _scheduledTask.IsRunning().Should().BeTrue("because the task is in the middle of running");
            _scheduledTask.IsFinishedRunning().Should().BeFalse("because the task is not yet finished running");
            _actionRan = true;
            return Result.Successful();
        }

        [Fact]
        public void StartTime_ShouldBeNullBeforeRunningTask()
        {
            var scheduledTask = CreateScheduledTask();
            scheduledTask.StartTime.Should().BeNull("because the task hasn't started yet");
        }

        [Fact]
        public void FinishTime_ShouldBeNullBeforeFinishingTask()
        {
            var scheduledTask = CreateScheduledTask();
            scheduledTask.CompleteTime.Should().BeNull("because the task has not yet finished");
        }

        [Fact]
        public void StartTime_ShouldBeClockTimeWhenStarted()
        {
            _clock = new FakeClock();
            _scheduledTask = CreateScheduledTask(AssertStartTimeMatchesClock, clock: _clock);

            _scheduledTask.Run();

            _actionRan.Should().BeTrue();
        }

        [Fact]
        public void CompleteTime_ShouldBeClockTimeAfterFinished()
        {
            _clock = new FakeClock();
            _scheduledTask = CreateScheduledTask(AssertStartTimeMatchesClock, clock: _clock);

            _scheduledTask.Run();

            _scheduledTask.CompleteTime.Should().Be(_clock.CurrentInstant.ToDateTimeUtc());
        }

        private Result AssertStartTimeMatchesClock()
        {
            _scheduledTask.StartTime.Should().Be(_clock.CurrentInstant.ToDateTimeUtc());
            _clock.AddToCurrentInstant(Duration.FromMinutes(5));
            _actionRan = true;
            return Result.Successful();
        }
    }
}