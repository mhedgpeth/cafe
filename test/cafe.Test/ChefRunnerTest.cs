using FluentAssertions;
using Xunit;

namespace cafe.Test
{
    public class ChefRunnerTest
    {
        [Fact]
        public void DetermineVersion_ShouldParseVersionFromOutput()
        {
            var process = new FakeChefProcess();
            process.LogEntriesToReceiveDuringRun.Add(ChefLogEntry.CreateMinimalEntry("Chef: 12.17.44"));

            var runner = new ChefRunner(() => process);

            runner.RetrieveVersion().Should().Be(new System.Version(12, 17, 44));
        }

        [Fact]
        public void ParseVersion_ShouldParseDifferentVersion()
        {
            ChefRunner.ParseVersion("Chef: 1.2.3").Should().Be(new System.Version(1, 2, 3));
        }
    }
}