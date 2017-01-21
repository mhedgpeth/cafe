using System;
using cafe.Shared;
using FluentAssertions;
using Xunit;

namespace cafe.Test.Shared
{
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
            status.ToString().Should().Be("Chef is paused");
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