using cafe.Server.Jobs;
using cafe.Shared;
using FluentAssertions;
using Xunit;

namespace cafe.Test.Shared
{
    public class JobRunnerStatusTest
    {
        [Fact]
        public void QueuedTasks_ShouldDefaultToEmpty()
        {
            new JobRunnerStatus().QueuedTasks.Should().BeEmpty("because bad things would happen if it were null");
        }

        [Fact]
        public void FinishedTasks_ShouldDefaultToEmpty()
        {
            new JobRunnerStatus().FinishedTasks.Should().BeEmpty("because bad things would happen if it were null");
        }

        [Fact]
        public void ToString_ShouldBeHelpful()
        {
            var serverStatus = new JobRunnerStatus();
            serverStatus.ToString()
                .Should()
                .Be($"Server has 0 queued tasks and 0 finished tasks");
        }

    }
}