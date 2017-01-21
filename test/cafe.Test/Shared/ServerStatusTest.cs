using System;
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

    public class ChefStatusTest
    {
        public static ChefStatus CreateStatusRunningEveryTwoSeconds()
        {
            return new ChefStatus()
            {
                IsRunning = true,
                Interval = TimeSpan.FromSeconds(2)
            };
        }

        [Fact]
        public void ToString_ShouldBeNotRunning()
        {
            var status = new ChefStatus() {IsRunning = false};
            status.ToString().Should().Be("Chef is not running");
        }

        [Fact]
        public void ToString_ShouldBeRunningWithoutInterval()
        {
            var status = new ChefStatus() {IsRunning = true};
            status.ToString().Should().Be("Chef is running on demand");
        }

        [Fact]
        public void ToString_ShouldBeRunningWithInterval()
        {
            CreateStatusRunningEveryTwoSeconds().ToString().Should().Be("Chef is running every 2 seconds");
        }
    }
}