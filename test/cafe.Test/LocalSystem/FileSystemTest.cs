using cafe.LocalSystem;
using cafe.Test.Chef;
using Xunit;
using FluentAssertions;

namespace cafe.Test.LocalSystem
{
    public class FileSystemTest
    {
        [Fact]
        public void FindChefInstallationDirectory_ShouldReturnNullIfNotFound()
        {
            const string path = @"C:\something";
            var fileSystem = new FileSystem(ChefProcessTest.CreateEnvironmentWithPath(path),
                new FakeFileSystemCommands());
            var actual = fileSystem.FindInstallationDirectoryInPathContaining("chef-client.bat");

            actual.Should().BeNull("because the file doesn't exist anywhere on the path");
        }

        [Fact]
         public void FindChefInstallationDirectory_ShouldFindChefFileThatExistsInPathEnvironmentVariable()
        {
            string chefPath = $@"{ChefProcessTest.ChefInstallPath}\bin";
            var path = $@"C:\something;C:\else;{chefPath}";
            var chefClientFileExclusively = @"chef-client.bat";
            var chefClientFile = $@"{chefPath}\{chefClientFileExclusively}";
            var fileSystem = new FileSystem(ChefProcessTest.CreateEnvironmentWithPath(path),
                FakeFileSystemCommands.CreateWithExistingFiles(chefClientFile));

            fileSystem.FindInstallationDirectoryInPathContaining(chefClientFileExclusively).Should()
                .Be(chefPath, "because it is the parent directory of the bin path in which the batch file exists");
        }


    }
}