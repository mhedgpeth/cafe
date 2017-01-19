using System;
using cafe.Server.Scheduling;
using FluentAssertions;
using NodaTime;
using Xunit;

namespace cafe.Test.Server.Scheduling
{
    public class RecurringTaskTest
    {
        private static readonly Duration FiveMinutes = Duration.FromMinutes(5);

        [Fact]
        public void IsReadyToRun_ShouldBeFalseWhenNotPastCreateTime()
        {
            var clock = new FakeClock();
            var recurringTask = CreateRecurringTask(clock, FiveMinutes, CreateFakeScheduledTask);

            recurringTask.IsReadyToRun.Should().BeFalse();
        }

        public static RecurringTask CreateRecurringTask(IClock clock = null, Duration? fiveMinutes = null, Func<RecurringTask, IScheduledTask> scheduledTaskCreator = null)
        {
            clock = clock ?? new FakeClock();
            Duration interval = fiveMinutes ?? FiveMinutes;
            scheduledTaskCreator = scheduledTaskCreator ?? CreateFakeScheduledTask;
            return new RecurringTask("task", clock, interval, scheduledTaskCreator);
        }

        [Fact]
        public void IsReadyToRun_ShouldBeTrueWhenPastCreateTime()
        {
            var clock = new FakeClock();

            var recurringTask = CreateRecurringTask(clock, FiveMinutes, CreateFakeScheduledTask);

            clock.AddToCurrentInstant(FiveMinutes);

            recurringTask.IsReadyToRun.Should().BeTrue();
        }

        [Fact]
        public void CreateScheduledTask_ShouldBeFalseAfterFirstRunButBeforeNextScheduled()
        {
            var clock = new FakeClock();
            var expected = new FakeScheduledTask();
            var recurringTask = CreateRecurringTask(clock, FiveMinutes, task => expected);
            clock.AddToCurrentInstant(FiveMinutes);
            var actual = recurringTask.ProvideNextScheduledTask();

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateScheduledTask_ShouldThrowExceptionWhenNotReady()
        {
            var recurringTask = CreateRecurringTask(new FakeClock(), FiveMinutes, CreateFakeScheduledTask);
            Assert.Throws<InvalidOperationException>(() => recurringTask.ProvideNextScheduledTask());
        }

        private static IScheduledTask CreateFakeScheduledTask(RecurringTask recurringTask)
        {
            return new FakeScheduledTask() { RecurringTask = recurringTask };
        }

        [Fact]
        public void IsReadyToRun_ShouldBeFalseAfterCreatingScheduleTaskBeforeNextDurationTime()
        {
            var clock = new FakeClock();
            var recurringTask = CreateRecurringTask(clock, FiveMinutes, CreateFakeScheduledTask);
            clock.AddToCurrentInstant(FiveMinutes);

            recurringTask.ProvideNextScheduledTask();

            recurringTask.IsReadyToRun.Should()
                .BeFalse(
                    "because a task for that time was already created, and the duration since that time hasn't been traversed");
        }

        [Fact]
        public void LastRun_ShouldDefaultToNullBeforeItRuns()
        {
            var recurringTask = CreateRecurringTask(new FakeClock(), FiveMinutes, CreateFakeScheduledTask);
            recurringTask.ToRecurringTaskStatus().LastRun.Should().BeNull("because the recurring task has not yet run");
        }

        [Fact]
        public void ExpectedNextRun_ShouldBeCreatedDatePlusDurationOnInitialRun()
        {
            var interval = FiveMinutes;
            var recurringTask = CreateRecurringTask(new FakeClock(), interval, CreateFakeScheduledTask);

            var recurringTaskStatus = recurringTask.ToRecurringTaskStatus();
            recurringTaskStatus.ExpectedNextRun.Should().Be(recurringTaskStatus.Created.Add(interval.ToTimeSpan()));
        }

        [Fact]
        public void ExpectedNextRun_ShouldBeLastRunDatePlusIntervalAfterItRuns()
        {
            var clock = new FakeClock();
            var interval = FiveMinutes;
            var recurringTask = CreateRecurringTask(clock, interval, CreateFakeScheduledTask);

            clock.AddToCurrentInstant(FiveMinutes);
            recurringTask.ProvideNextScheduledTask();

            var status = recurringTask.ToRecurringTaskStatus();
            status.ExpectedNextRun.Should().Be(status.LastRun.Value.Add(interval.ToTimeSpan()));
        }

        [Fact]
        public void Pause_ShouldMakeTheTaskNotReadyEvenWhenItIsDue()
        {
            var clock = new FakeClock();
            var interval = FiveMinutes;
            var recurringTask = CreateRecurringTask(clock, interval, CreateFakeScheduledTask);

            clock.AddToCurrentInstant(interval);

            recurringTask.Pause();

            recurringTask.IsReadyToRun.Should().BeFalse("because the task is paused");
        }

        [Fact]
        public void Resume_ShouldMakeTheTaskRunnableAgain()
        {
            var clock = new FakeClock();
            var interval = FiveMinutes;
            var recurringTask = CreateRecurringTask(clock, interval, CreateFakeScheduledTask);

            clock.AddToCurrentInstant(interval);

            recurringTask.Pause();
            recurringTask.Resume();

            recurringTask.IsReadyToRun.Should().BeTrue("because the task resumed after pausing");
        }

        [Fact]
        public void RunTaskImmediately_ShouldMakeItReady()
        {
            var recurringTask = CreateRecurringTask(new FakeClock(), FiveMinutes, CreateFakeScheduledTask);
            var task = new FakeScheduledTask();
            recurringTask.RunTaskImmediately(task);

            recurringTask.IsReadyToRun.Should().BeTrue("there is a task that is supposed to run immediately");
            recurringTask.ProvideNextScheduledTask()
                .Should()
                .BeSameAs(task, "because this is the task that should run immediately");
        }

        [Fact]
        public void ProvideNextScheduledTask_ShouldProvideNewScheduledTaskAfterRunningImmediately()
        {
            var newTask = new FakeScheduledTask();
            var clock = new FakeClock();
            var recurringTask = CreateRecurringTask(clock, FiveMinutes, task => newTask);

            recurringTask.RunTaskImmediately(new FakeScheduledTask());
            recurringTask.ProvideNextScheduledTask();

            recurringTask.IsReadyToRun.Should().BeFalse("because the clock hasn't moved and we are not yet ready");

            clock.AddToCurrentInstant(FiveMinutes);

            recurringTask.IsReadyToRun.Should().BeTrue("because five minutes have passed");
            recurringTask.ProvideNextScheduledTask()
                .Should()
                .BeSameAs(newTask, "because we should be creating new tasks after a task was run immediately.");
        }

    }
}