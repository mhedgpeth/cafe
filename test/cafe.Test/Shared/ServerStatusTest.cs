using cafe.Shared;
using FluentAssertions;
using Xunit;

namespace cafe.Test.Shared
{
    public class ServerStatusTest
    {
        [Fact]
        public void QueuedTasks_ShouldDefaultToEmpty()
        {
            new ServerStatus().QueuedTasks.Should().BeEmpty("because bad things would happen if it were null");
        }

        [Fact]
        public void FinishedTasks_ShouldDefaultToEmpty()
        {
            new ServerStatus().FinishedTasks.Should().BeEmpty("because bad things would happen if it were null");
        }

        [Fact]
        public void ToString_ShouldBeHelpful()
        {
            var serverStatus = new ServerStatus {ChefStatus = ChefStatusTest.CreateStatusRunningEveryTwoSeconds()};
            serverStatus.ToString()
                .Should()
                .Be($"Server has 0 queued tasks and 0 finished tasks and {serverStatus.ChefStatus}");
        }

    }
}