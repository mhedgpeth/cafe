using System;
using cafe.Server;
using Xunit;
using FluentAssertions;
using NodaTime;

namespace cafe.Test.Server
{
    public class SchedulerTest
    {
        [Fact]
        public void ProcessTasks_ShouldDoNothingIfNoTasksAreScheduled()
        {
            var scheduler = new Scheduler();
            scheduler.ProcessTasks();
            // nothing happened, good!
        }

        [Fact]
        public void Schedule_ShouldRunTaskImmediatelyIfThereAreNone()
        {
            var scheduler = new Scheduler();
            var task = new FakeScheduledTask();
            scheduler.Schedule(task);
            scheduler.ProcessTasks();
            task.WasRunCalled.Should().BeTrue();
        }

        [Fact]
        public void Schedule_ShouldNotRunWhenAnotherTask()
        {
            var scheduler = new Scheduler();
            var longRunningTask = new FakeScheduledTask() {FinishTaskImmediately = false};
            var anotherTask = new FakeScheduledTask();
            scheduler.Schedule(longRunningTask);
            scheduler.ProcessTasks();
            scheduler.Schedule(anotherTask);
            scheduler.ProcessTasks();
            scheduler.ProcessTasks();

            anotherTask.WasRunCalled.Should().BeFalse();
        }

        [Fact]
        public void ProcessTasks_ShouldScheduleAndExecuteReadyRecurringTask()
        {
            var scheduledTask = new FakeScheduledTask();
            var clock = new FakeClock();
            var fiveMinutes = Duration.FromMinutes(5);
            var recurringTask = new RecurringTask(clock, fiveMinutes, () => scheduledTask);
            clock.AddToCurrentInstant(fiveMinutes);

            var scheduler = new Scheduler();
            scheduler.Add(recurringTask);

            scheduler.ProcessTasks();
            scheduler.ProcessTasks();

            scheduledTask.WasRunCalled.Should().BeTrue("because the recurring task was due, it should have created a scheduled task and run it");
        }

        private FakeScheduledTask CreateRecurringTask()
        {
            return new FakeScheduledTask();
        }
    }
}