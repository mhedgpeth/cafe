using System;
using Xunit;
using FluentAssertions;

namespace cafe.Test
{
    public class ChefProcessTest
    {
        private const string ChefInstallPath = @"C:\opscode\chef";

        [Fact]
        public void FindChefInstallationDirectory_ShouldFindChefFileThatExistsInPathEnvironmentVariable()
        {
            string chefPath = $@"{ChefInstallPath}\bin";
            var path = $@"C:\something;C:\else;{chefPath}";
            Func<string, bool> fileExists = p => p == chefPath;
            var actual = ChefProcess.FindChefInstallationDirectory(path, fileExists);

            actual.Should()
                .Be(ChefInstallPath, "because it is the parent directory of the bin path in which the batch file exists");
        }

        [Fact]
        public void FindChefInstallationDirectory_ShouldReturnNullIfNotFound()
        {
            const string path = @"C:\something";
            var actual = ChefProcess.FindChefInstallationDirectory(path, s => false);

            actual.Should().BeNull("because the file doesn't exist anywhere on the path");
        }

        [Fact]
        public void RubyExecutable_ShouldBeInEmbeddedDirectory()
        {
            var actual = ChefProcess.RubyExecutableWithin(ChefInstallPath);

            actual.Should().Be($@"{ChefInstallPath}\embedded\bin\ruby.exe");
        }

        [Fact]
        public void ChefClientLoaderWithin_ShouldBeInBinDirectory()
        {
            ChefProcess.ChefClientLoaderWithin(ChefInstallPath).Should().Be($@"{ChefInstallPath}\bin\chef-client");
        }

        [Fact]
        public void NullOutput_ShouldNotBeSharedAsLogEntry()
        {
            var process = new FakeProcess();
            var chefProcess = new ChefProcess(() => process);
            process.OutputDataReceivedDuringWaitForExit.Add(null);
            AssertChefProcessRunShouldNotGenerateLogEntries(chefProcess);
        }

        private static void AssertChefProcessRunShouldNotGenerateLogEntries(ChefProcess chefProcess)
        {
            bool logEntryReceived = false;
            chefProcess.LogEntryReceived +=
                (sender, entry) => { logEntryReceived = true; };
            chefProcess.Run();
            logEntryReceived.Should().BeFalse("because null entries should not be shared");
        }

        [Fact]
        public void NullErrors_ShouldNotBeSharedAsLogEntry()
        {
            var process = new FakeProcess();
            var chefProcess = new ChefProcess(() => process);
            process.ErrorDataReceivedDuringWaitForExit.Add(null);
            AssertChefProcessRunShouldNotGenerateLogEntries(chefProcess);
        }
    }
}