using System;
using cafe.Server.Scheduling;
using FluentAssertions;
using NodaTime;
using Xunit;

namespace cafe.Test.Server.Scheduling
{
    public class SchedulerTest
    {
        [Fact]
        public void ProcessTasks_ShouldDoNothingIfNoTasksAreScheduled()
        {
            var scheduler = CreateScheduler();
            scheduler.ProcessTasks();
            // nothing happened, good!
        }

        private static Scheduler CreateScheduler()
        {
            return new Scheduler(new FakeTimerFactory());
        }

        [Fact]
        public void Schedule_ShouldRunTaskImmediatelyIfThereAreNone()
        {
            var scheduler = CreateScheduler();
            var task = new FakeScheduledTask();
            scheduler.Schedule(task);
            scheduler.ProcessTasks();
            task.WasRunCalled.Should().BeTrue();
        }

        [Fact]
        public void Schedule_ShouldNotRunWhenAnotherTask()
        {
            var scheduler = CreateScheduler();
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

            var scheduler = CreateScheduler();
            scheduler.Add(recurringTask);

            scheduler.ProcessTasks();
            scheduler.ProcessTasks();

            scheduledTask.WasRunCalled.Should().BeTrue("because the recurring task was due, it should have created a scheduled task and run it");
        }

        private FakeScheduledTask CreateRecurringTask()
        {
            return new FakeScheduledTask();
        }

        [Fact]
        public void TimerAction_ShouldProcessTasks()
        {
            var timerFactory = new FakeTimerFactory();
            var scheduler = new Scheduler(timerFactory);
            var task = new FakeScheduledTask();
            scheduler.Schedule(task);

            timerFactory.FireTimerAction();

            task.WasRunCalled.Should().BeTrue("because the timer fired which should process tasks");
        }

        [Fact]
        public void Pause_ShouldPauseTimer()
        {
            var timerFactory = new FakeTimerFactory();
            var scheduler = new Scheduler(timerFactory);
            var task = new FakeScheduledTask();
            scheduler.Schedule(task);
            scheduler.Pause();

            timerFactory.FireTimerAction();

            task.WasRunCalled.Should().BeFalse("because the scheduler pausing not process any tasks");
        }

        [Fact]
        public void IsRunning_ShouldBeTrueByDefault()
        {
            CreateScheduler().IsRunning.Should().BeTrue("because the scheduler defaults to a running state");
        }

        [Fact]
        public void IsRunning_ShouldBeFalseWhenPaused()
        {
            var scheduler = CreateScheduler();
            scheduler.Pause();
            scheduler.IsRunning.Should().BeFalse("because the scheduler is paused");
        }

        [Fact]
        public void FindStatusById_ShouldFindQueuedStatus()
        {
            var scheduler = CreateScheduler();
            var task = new FakeScheduledTask();
            scheduler.Schedule(task);

            var status = scheduler.FindStatusById(task.Id);

            status.Should().Be(task.ToTaskStatus());
        }

        [Fact]
        public void FindStatusById_ShouldFindFinishedTask()
        {
            var scheduler = CreateScheduler();
            var task = new FakeScheduledTask();
            scheduler.Schedule(task);
            scheduler.ProcessTasks();
            scheduler.ProcessTasks();

            var status = scheduler.FindStatusById(task.Id);

            status.Should().Be(task.ToTaskStatus());
        }

        [Fact]
        public void FindStatusById_ShouldReturnNullIfTaskNotFound()
        {
            CreateScheduler()
                .FindStatusById(Guid.NewGuid())
                .Should()
                .BeNull("because a task with that id hasn't yet been scheduled");
        }
    }
}