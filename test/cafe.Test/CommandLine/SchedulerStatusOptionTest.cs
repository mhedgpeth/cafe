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
            var scheduler = new Mock<IChefServer>();
            scheduler.Setup(s => s.GetStatus()).Throws<HttpRequestException>();

            var option = new StatusOption(() => scheduler.Object);

            var result = option.Run();

            result.IsSuccess.Should().BeFalse("because the task should fail when the server can't be connected to");
        }
    }
}