using System;
using cafe.Server.Jobs;
using cafe.Server.Scheduling;
using cafe.Test.Server.Scheduling;
using FluentAssertions;
using Xunit;

namespace cafe.Test.Server.Jobs
{
    public class JobRunnerTest
    {
        [Fact]
        public void ProcessTasks_ShouldDoNothingIfNoTasksAreScheduled()
        {
            var runner = CreateJobRunner();
            runner.ProcessQueue();
            // nothing happened, good!
        }

        [Fact]
        public void Schedule_ShouldRunTaskImmediatelyIfThereAreNone()
        {
            var runner = CreateJobRunner();
            var jobRun = new FakeJobRun();

            runner.Enqueue(jobRun);
            runner.ProcessQueue();

            jobRun.WasRunCalled.Should().BeTrue();
        }

        [Fact]
        public void ProcessQueue_ShouldNotRunWhenAnotherTaskIsRunning()
        {
            var runner = CreateJobRunner();
            var longRunningTask = new FakeJobRun() {FinishTaskImmediately = false};
            var anotherTask = new FakeJobRun();
            runner.Enqueue(longRunningTask);
            runner.ProcessQueue();
            runner.Enqueue(anotherTask);
            runner.ProcessQueue();
            runner.ProcessQueue();

            anotherTask.WasRunCalled.Should().BeFalse();
        }


        [Fact]
        public void TimerAction_ShouldProcessTasks()
        {
            var timerFactory = new FakeTimerFactory();
            var runner = CreateJobRunner(timerFactory: timerFactory);
            var task = new FakeJobRun();
            runner.Enqueue(task);

            timerFactory.FireTimerAction();

            task.WasRunCalled.Should().BeTrue("because the timer fired which should process tasks");
        }

        public static JobRunner CreateJobRunner(IActionExecutor actionExecutor = null, ITimerFactory timerFactory = null)
        {
            timerFactory = timerFactory ?? new FakeTimerFactory();
            actionExecutor = actionExecutor ?? new FakeActionExecutor();
            return new JobRunner(timerFactory, actionExecutor);
        }

        [Fact]
        public void FindStatusById_ShouldFindQueuedStatus()
        {
            var runner = CreateJobRunner();
            var jobRun = new FakeJobRun();
            runner.Enqueue(jobRun);

            var status = runner.FindStatusById(jobRun.Id);

            status.Id.Should().Be(jobRun.Id);
        }

        [Fact]
        public void FindStatusById_ShouldFindFinishedTask()
        {
            var runner = CreateJobRunner();
            var task = new FakeJobRun();
            runner.Enqueue(task);
            runner.ProcessQueue();
            runner.ProcessQueue();

            var status = runner.FindStatusById(task.Id);

            status.Id.Should().Be(task.Id);
        }

        [Fact]
        public void FindStatusById_ShouldReturnNullIfTaskNotFound()
        {
            CreateJobRunner()
                .FindStatusById(Guid.NewGuid())
                .Should()
                .BeNull("because a task with that id hasn't yet been scheduled");
        }

        [Fact]
        public void ProcessTasks_ShouldExecuteTaskInBackground()
        {
            var actionExecutor = new FakeActionExecutor();
            var runner = CreateJobRunner(actionExecutor);
            var scheduledTask = new FakeJobRun();

            runner.Enqueue(scheduledTask);
            runner.ProcessQueue();
            runner.ProcessQueue();

            actionExecutor.WasExecuted.Should()
                .BeTrue(
                    "because the scheduler should use the action executor to run its tasks so in the real program they'll run in the background");
        }

        [Fact]
        public void Schedule_ShouldImmediatelyProcessTasks()
        {
            var task = new FakeJobRun();
            var runner = CreateJobRunner();

            runner.Enqueue(task);

            task.WasRunCalled.Should()
                .BeTrue(
                    "because scheduling a scheduled task should immediately process it to make manually submitted tasks faster");
        }

    }
}