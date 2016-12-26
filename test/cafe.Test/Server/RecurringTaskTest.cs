using System;
using cafe.Server;
using Xunit;
using NodaTime;
using FluentAssertions;

namespace cafe.Test.Server
{
    public class RecurringTaskTest
    {
        private static readonly Duration FiveMinutes = Duration.FromMinutes(5);

        [Fact]
        public void IsReadyToRun_ShouldBeFalseWhenNotPastCreateTime()
        {
            var clock = new MockClock();
            var recurringTask = new RecurringTask(clock, FiveMinutes, CreateFakeScheduledTask);

            recurringTask.IsReadyToRun.Should().BeFalse();
        }

        [Fact]
        public void IsReadyToRun_ShouldBeTrueWhenPastCreateTime()
        {
            var clock = new MockClock();

            var recurringTask = new RecurringTask(clock, FiveMinutes, CreateFakeScheduledTask);

            clock.AddToCurrentInstant(FiveMinutes);

            recurringTask.IsReadyToRun.Should().BeTrue();
        }

        [Fact]
        public void CreateScheduledTask_ShouldBeFalseAfterFirstRunButBeforeNextScheduled()
        {
            var clock = new MockClock();
            var expected = new FakeScheduledTask();
            var recurringTask = new RecurringTask(clock, FiveMinutes, () => expected);
            clock.AddToCurrentInstant(FiveMinutes);
            var actual = recurringTask.CreateScheduledTask();

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateScheduledTask_ShouldThrowExceptionWhenNotReady()
        {
            var recurringTask = new RecurringTask(new MockClock(), FiveMinutes, CreateFakeScheduledTask);
            Assert.Throws<InvalidOperationException>(() => recurringTask.CreateScheduledTask());
        }

        private static IScheduledTask CreateFakeScheduledTask()
        {
            return new FakeScheduledTask();
        }

        [Fact]
        public void IsReadyToRun_ShouldBeFalseAfterCreatingScheduleTaskBeforeNextDurationTime()
        {
            var clock = new MockClock();
            var recurringTask = new RecurringTask(clock, FiveMinutes, CreateFakeScheduledTask);
            clock.AddToCurrentInstant(FiveMinutes);

            recurringTask.CreateScheduledTask();

            recurringTask.IsReadyToRun.Should()
                .BeFalse(
                    "because a task for that time was already created, and the duration since that time hasn't been traversed");
        }
    }
}