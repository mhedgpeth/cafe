using cafe.Client;
using cafe.Options;
using cafe.Test.LocalSystem;
using FluentAssertions;
using Moq;
using Xunit;

namespace cafe.Test.Options
{
    public class BootstrapChefPolicyOptionTest
    {
        [Fact]
        public void CanParse_ShouldBeTrueForValidArguments()
        {
            var option = new BootstrapChefPolicyOption(new Mock<IClientFactory>().Object,
                new Mock<ISchedulerWaiter>().Object, new FakeFileSystemCommands());
            option.IsSatisfiedBy("chef", "bootstrap", "policy:", "webserver", "group:", "qa", "config:", "client.rb",
                "validator:", "validator.pem").Should().BeTrue();
        }
    }
}