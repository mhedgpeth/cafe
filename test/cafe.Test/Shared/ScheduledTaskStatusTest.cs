using System;
using cafe.Shared;
using FluentAssertions;
using NodaTime;
using Xunit;

namespace cafe.Test.Shared
{
    public class ScheduledTaskStatusTest
    {
        [Fact]
        public void Copy_ShouldCopyAllElements()
        {
            var status = CreateFullStatus();

            var copy = status.Copy();

            copy.Id.Should().Be(status.Id);
            copy.StartTime.Should().Be(status.StartTime);
            copy.CompleteTime.Should().Be(status.CompleteTime);
            copy.Description.Should().Be(status.Description);
            copy.State.Should().Be(status.State);
            copy.Result.Should().Be(status.Result);
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
            status.CompleteTime = status.CompleteTime.Value.Add(TimeSpan.FromMinutes(2));

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

        private static ScheduledTaskStatus CreateFullStatus()
        {
            var startTime = Instant.FromUtc(2016, 12, 27, 11, 15).ToDateTimeUtc();
            return new ScheduledTaskStatus
            {
                Id = new Guid("9eb4a43d-306d-44e2-82fe-188813518fdd"),
                StartTime = startTime,
                CompleteTime = startTime.Add(TimeSpan.FromMinutes(5)),
                Description = "a task for testing",
                State = TaskState.Finished,
                Result = Result.Failure("something bad happened!")
            };
        }
    }
}