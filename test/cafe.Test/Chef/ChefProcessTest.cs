using System;
using cafe.Chef;
using cafe.CommandLine.LocalSystem;
using cafe.LocalSystem;
using cafe.Test.LocalSystem;
using FluentAssertions;
using Xunit;
using ProcessExecutor = cafe.CommandLine.LocalSystem.ProcessExecutor;

namespace cafe.Test.Chef
{
    public class ChefProcessTest
    {
        public const string ChefInstallPath = @"C:\opscode\chef";



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
            var chefProcess = CreateChefProcess(() => process);
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
            var chefProcess = CreateChefProcess(() => process);
            process.ErrorDataReceivedDuringWaitForExit.Add(null);
            AssertChefProcessRunShouldNotGenerateLogEntries(chefProcess);
        }


        private ChefProcess CreateChefProcess(Func<FakeProcess> processCreator = null, IFileSystem fileSystem = null, IEnvironment environment = null)
        {
            var defaultChefDirectory = @"C:\opscode\chef\bin";
            processCreator = processCreator ?? (() => new FakeProcess());
            fileSystem = fileSystem ?? new FakeFileSystem(); // FakeFileSystem.CreateWithExistingFiles($@"{defaultChefDirectory}\chef-client.bat");
            environment = environment ?? CreateEnvironmentWithPath(defaultChefDirectory);
            return new ChefProcess(new ProcessExecutor(processCreator), new FakeFileSystem());
        }

        public static FakeEnvironment CreateEnvironmentWithPath(string path)
        {
            var environment = new FakeEnvironment();
            environment.EnvironmentVariables.Add("PATH", path);
            return environment;
        }
    }
}