using System;
using cafe.Shared;
using FluentAssertions;
using NodaTime;
using Xunit;

namespace cafe.Test.Shared
{
    public class JobRunStatusTest
    {
        [Fact]
        public void Copy_ShouldCopyAllElements()
        {
            var status = CreateFullStatus();

            var copy = status.Copy();

            copy.Id.Should().Be(status.Id);
            copy.StartTime.Should().Be(status.StartTime);
            copy.FinishTime.Should().Be(status.FinishTime);
            copy.Description.Should().Be(status.Description);
            copy.State.Should().Be(status.State);
            copy.Result.Should().Be(status.Result);
            copy.CurrentMessage.Should().Be(status.CurrentMessage);
        }

        [Fact]
        public void Equals_ShouldBeFalseWhenStartTimeDiffers()
        {
            var status = CreateFullStatus();
            status.StartTime = status.StartTime.Value.Subtract(TimeSpan.FromMinutes(2));

            status.Should().NotBe(CreateFullStatus(), "because start time differs, the two should not be equal");
        }

        [Fact]
        public void Equals_ShouldBeFalseWhenCompleteTimeDiffers()
        {
            var status = CreateFullStatus();
            status.FinishTime = status.FinishTime.Value.Add(TimeSpan.FromMinutes(2));

            status.Should().NotBe(CreateFullStatus(), "because complete time differs, the two should not be equal");
        }

        [Fact]
        public void Equals_ShouldBeFalseWhenResultIsDifferent()
        {
            var status = CreateFullStatus();
            status.Result = Result.Successful();

            status.Should().NotBe(CreateFullStatus());
        }

        [Fact]
        public void Equals_ShouldBeTrueWhenEqual()
        {
            CreateFullStatus().Should().Be(CreateFullStatus(), "because all values are equal, the two should be equal");
        }
        private static readonly DateTime StartTime = Instant.FromUtc(2016, 12, 27, 11, 15).ToDateTimeUtc();

        private static JobRunStatus CreateFullStatus()
        {
            return new JobRunStatus
            {
                Id = new Guid("9eb4a43d-306d-44e2-82fe-188813518fdd"),
                StartTime = StartTime,
                FinishTime = StartTime.Add(TimeSpan.FromMinutes(5)),
                Description = "a task for testing",
                State = JobRunState.Finished,
                CurrentMessage = "Task finisehd!",
                Result = Result.Failure("something bad happened!")
            };
        }

        [Fact]
        public void ToString_ShouldBeDescriptiveForNotRun()
        {
            var status = JobRunStatus.Create("do something");
            status.ToString().Should().Be($"Task {status.Description} ({status.Id}) - Not yet run");
        }

        [Fact]
        public void ToString_ShouldBeDescriptiveForRunning()
        {
            var status = JobRunStatus.Create("do something").ToRunningState(StartTime);
            status.ToString()
                .Should()
                .Be($"Task {status.Description} ({status.Id}) - Running for {(int)status.Duration.Value.TotalSeconds} seconds");
        }

        [Fact]
        public void ToString_ShouldBeDescriptiveForFinished()
        {
            var status = JobRunStatus.Create("do something")
                .ToRunningState(StartTime)
                .ToFinishedState(Result.Failure("failed!"), StartTime.Add(TimeSpan.FromSeconds(5)));

            status.ToString().Should().Be($"Task {status.Description} ({status.Id}) - {status.Result}");
        }

        [Fact]
        public void Duration_ShouldBeNullBeforeRunning()
        {
            var status = JobRunStatus.Create("do something");

            status.Duration.Should().BeNull("because it hasn't yet started");
        }

        [Fact]
        public void Duration_ShouldBeBasedOnCurrentDate()
        {
            var status = JobRunStatus.Create("do something")
                .ToRunningState(DateTime.Now.Subtract(TimeSpan.FromMinutes(1)));

            status.Duration.HasValue.Should().BeTrue("because the task has started");
            status.Duration.Value.TotalSeconds.Should().BeGreaterOrEqualTo(60);
        }

        [Fact]
        public void Duration_ShouldBeBasedOnStartAndCompleteDate()
        {

            var status = JobRunStatus.Create("do something")
                .ToRunningState(StartTime)
                .ToFinishedState(Result.Successful(), StartTime.AddMinutes(2));

            status.Duration.HasValue.Should().BeTrue("because the task has finished");
            status.Duration.Value.TotalSeconds.Should().Be(120);
        }
    }
}