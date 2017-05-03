using System.Net.Http;
using cafe.Client;
using cafe.Options;
using FluentAssertions;
using Moq;
using Xunit;

namespace cafe.Test.CommandLine
{
    public class SchedulerStatusOptionTest
    {
        [Fact]
        public void Exception_ShouldNotBeRethrown()
        {
            var jobServer = new Mock<IJobServer>();
            jobServer.Setup(s => s.GetStatus()).Throws<HttpRequestException>();

            var option = new StatusOption(() => jobServer.Object);

            var result = option.Run();

            result.IsSuccess.Should().BeFalse("because the task should fail when the server can't be connected to");
        }
    }
}