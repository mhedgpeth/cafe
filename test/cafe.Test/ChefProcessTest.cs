using System;
using System.Collections.Generic;
using cafe.Chef;
using cafe.LocalSystem;
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
            var actual =
                CreateChefProcess(fileSystem: FakeFileSystem.CreateWithExistingFiles($@"{chefPath}\chef-client.bat"), environment: CreateEnvironmentWithPath(path))
                    .FindChefInstallationDirectory();

            actual.Should()
                .Be(ChefInstallPath, "because it is the parent directory of the bin path in which the batch file exists");
        }

        [Fact]
        public void FindChefInstallationDirectory_ShouldReturnNullIfNotFound()
        {
            const string path = @"C:\something";
            var actual = CreateChefProcess(fileSystem: new FileSystem(), environment: CreateEnvironmentWithPath(path)).FindChefInstallationDirectory();

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
            fileSystem = fileSystem ?? FakeFileSystem.CreateWithExistingFiles($@"{defaultChefDirectory}\chef-client.bat");
            environment = environment ?? CreateEnvironmentWithPath(defaultChefDirectory);
            return new ChefProcess(processCreator, fileSystem, environment);
        }

        private FakeEnvironment CreateEnvironmentWithPath(string path)
        {
            var environment = new FakeEnvironment();
            environment.EnvironmentVariables.Add("PATH", path);
            return environment;
        }
    }

    public class FakeFileSystem : IFileSystem
    {
        public void EnsureDirectoryExists(string directory)
        {
        }

        public bool FileExists(string filename)
        {
            return ExistingFiles.Contains(filename);
        }

        public static FakeFileSystem CreateWithExistingFiles(params string[] files)
        {
            return new FakeFileSystem() {ExistingFiles = new List<string>(files)};
        }

        public List<string> ExistingFiles { get; set; } = new List<string>();
    }

    public class FakeEnvironment : IEnvironment
    {
        public string GetEnvironmentVariable(string key)
        {
            return EnvironmentVariables[key];
        }

        public IDictionary<string, string> EnvironmentVariables { get; }
        = new Dictionary<string, string>();
    }
}