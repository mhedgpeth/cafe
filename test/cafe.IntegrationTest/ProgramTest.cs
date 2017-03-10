using cafe.Client;
using cafe.CommandLine;
using cafe.LocalSystem;
using cafe.Options.Chef;
using cafe.Options.Server;
using cafe.Test.Chef;
using cafe.Test.Client;
using cafe.Test.LocalSystem;
using cafe.Test.Server.Scheduling;
using FluentAssertions;
using Moq;
using Xunit;

namespace cafe.IntegrationTest
{
    public class ProgramTest
    {
        [Fact]
        public void CreateRootGroup_ShouldParseBootstrapOption()
        {
            var root = CreateRootOptionGroup();

            var arguments = root.ParseArguments("chef", "bootstrap", "policy:", "cafe-demo", "group:", "qa", "config:",
                @"C:\Users\mhedg\.chef\client.rb", "validator:", @"C:\Users\mhedg\.chef\cafe-demo-validator.pem");

            arguments.Should().NotBeNull("because the arguments do match valid arguments for the application");

            var option = root.FindOption(arguments);

            option.Should().NotBeNull("because the arguments match");
            option.Should().BeAssignableTo<BootstrapChefPolicyOption>();
        }

        private static OptionGroup CreateRootOptionGroup()
        {
            var processExecutor = new ProcessExecutor(() => new FakeProcess());
            var fakeFileSystem = new FakeFileSystem();
            var root = Program.CreateRootGroup(new Mock<IClientFactory>().Object, new Mock<ISchedulerWaiter>().Object,
                new FakeFileSystemCommands(), processExecutor, fakeFileSystem,
                new ServiceStatusWaiter("waiter", new FakeAutoResetEvent(), new FakeTimerFactory(),
                    new ServiceStatusProvider(processExecutor, fakeFileSystem)), new FakeEnvironment());
            return root;
        }

        [Fact]
        public void CreateRootGroup_ShouldParseStatusOnOption()
        {
            var root = CreateRootOptionGroup();
            var arguments = root.ParseArguments("chef", "status", "on:", "localhost");

            arguments.Should().NotBeNull("because they should match the chef status");
        }

        [Fact]
        public void ParseArguments_ShouldReturnNullIfDownloadVersionNotGiven()
        {
            var root = CreateRootOptionGroup();
            var arguments = root.ParseArguments("chef", "download");

            arguments.Should().BeNull("because the version wasn't specified");
        }
    }
}