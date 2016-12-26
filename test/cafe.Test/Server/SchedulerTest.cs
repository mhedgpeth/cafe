using System;
using cafe.Server;
using Xunit;
using FluentAssertions;

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

        private FakeScheduledTask CreateRecurringTask()
        {
            return new FakeScheduledTask();
        }
    }
}