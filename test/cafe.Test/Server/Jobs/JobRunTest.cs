using System;
using cafe.CommandLine;
using cafe.Server.Jobs;
using cafe.Shared;
using cafe.Test.Shared;
using FluentAssertions;
using NodaTime;
using Xunit;

namespace cafe.Test.Server.Jobs
{
    public class JobRunTest
    {
        private JobRun _jobRun;
        private bool _actionRan;
        private FakeClock _clock;

        [Fact]
        public void Run_ShouldFireRunReadyEvent()
        {
            var expectedRun = CreateJobRun();
            var job = new SimpleJob(expectedRun);
            JobRun actualRun = null;
            job.RunReady += (sender, run) => actualRun = run;

            job.Run();

            actualRun.Should().BeSameAs(expectedRun, "because the job should create a new run and then fire an event");
        }


        public static JobRun CreateJobRun(string description = null, Func<IMessagePresenter, Result> action = null,
            IClock clock = null)
        {
            action = action ?? (presenter => Result.Successful());
            clock = clock ?? new FakeClock();
            return new JobRun(description, action, clock);
        }


        [Fact]
        public void Run_ShouldSetIsRunningToTrue()
        {
            _actionRan = false;
            _jobRun = CreateJobRun(action: AssertIsRunning);

            _jobRun.Run();

            _actionRan.Should().BeTrue("because the scheduled task ran");
        }

        private Result AssertIsRunning(IMessagePresenter presenter)
        {
            _jobRun.IsRunning.Should().BeTrue("because the task is in the middle of running");
            _jobRun.IsFinishedRunning.Should().BeFalse("because the task is not yet finished running");
            _actionRan = true;
            return Result.Successful();
        }

        [Fact]
        public void Run_ShouldCompleteProperly()
        {
            var jobRun = CreateJobRun();
            jobRun.Run();

            jobRun.IsFinishedRunning.Should().BeTrue("because the task has finisehd");
            jobRun.IsRunning.Should().BeFalse("because the task is no longer running");
        }

        [Fact]
        public void Id_ShouldBePopulatedAndConsistent()
        {
            var jobRun = CreateJobRun();

            jobRun.Id.Should().Be(jobRun.Id);
        }

        [Fact]
        public void ToString_ShouldContainDescription()
        {
            const string description = "a great task";
            var scheduledTask = CreateJobRun(description: description);

            scheduledTask.ToString().Should().Contain(description, "because this was the description given");
        }

        [Fact]
        public void StartTime_ShouldBeNullBeforeRunningTask()
        {
            var jobRun = CreateJobRun();
            jobRun.Start.Should().BeNull("because the task hasn't started yet");
        }

        [Fact]
        public void Finish_ShouldBeNullBeforeFinishingTask()
        {
            var jobRun = CreateJobRun();
            jobRun.Finish.Should().BeNull("because the task has not yet finished");
        }

        [Fact]
        public void Start_ShouldBeClockTimeWhenStarted()
        {
            _clock = new FakeClock();
            _jobRun = CreateJobRun(action: AssertStartMatchesClock, clock: _clock);

            _jobRun.Run();

            _actionRan.Should().BeTrue();
        }

        [Fact]
        public void Finish_ShouldBeClockTimeAfterFinished()
        {
            _clock = new FakeClock();
            _jobRun = CreateJobRun(action: AssertStartMatchesClock, clock: _clock);

            _jobRun.Run();

            _jobRun.Finish.Should().Be(_clock.CurrentInstant);
        }

        [Fact]
        public void ShowMessage_ShouldUpdateCurrentMessageInStatus()
        {
            var jobRun = CreateJobRun();
            const string message = "this is what we're currently doing!";

            jobRun.ShowMessage(message);

            jobRun.CurrentMessage.Should().Be(message);
        }


        private Result AssertStartMatchesClock(IMessagePresenter presenter)
        {
            _jobRun.Start.Should().Be(_clock.CurrentInstant);
            _clock.AddToCurrentInstant(Duration.FromMinutes(5));
            _actionRan = true;
            return Result.Successful();
        }

        [Fact]
        public void Exceptions_ShouldBeConvertedIntoFailures()
        {
            var ex = new Exception("this is not expected!");
            var task = CreateJobRun(action: presenter => { throw ex; });

            task.Run();

            task.Result.IsSuccess.Should().BeFalse("because an exception was thrown");
        }

        [Fact]
        public void ToStatus_ShouldBeConsistentWithJobRunState()
        {
            var jobRun = CreateJobRun();

            var status = jobRun.ToStatus();

            status.Id.Should().Be(jobRun.Id, "because the ids should match; they represent the same thing");
        }

        [Fact]
        public void Run_ShouldPassWhenActionPasses()
        {
            AssertRunActionShouldMatchResult(Result.Successful());
        }

        [Fact]
        public void Run_ShouldFailWhenActionFails()
        {
            AssertRunActionShouldMatchResult(Result.Failure("Failed!"));
        }

        private void AssertRunActionShouldMatchResult(Result result)
        {
            var jobRun = CreateJobRun(action: presenter => result);

            jobRun.Run();

            jobRun.Result.Should()
                .BeSameAs(result, $"because the underlying action result was {result}");
        }
    }
}