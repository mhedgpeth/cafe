using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace cafe.Test
{
    public class ChefRunnerTest
    {
        private const string ChefInstallPath = @"C:\opscode\chef";

        [Fact]
        public void FindChefInstallationDirectory_ShouldFindChefFileThatExistsInPathEnvironmentVariable()
        {
            string chefPath = $@"{ChefInstallPath}\bin";
            var path = $@"C:\something;C:\else;{chefPath}";
            Func<string, bool> fileExists = p => p == chefPath;
            var actual = ChefRunner.FindChefInstallationDirectory(path, fileExists);

            actual.Should()
                .Be(ChefInstallPath, "because it is the parent directory of the bin path in which the batch file exists");
        }

        [Fact]
        public void FindChefInstallationDirectory_ShouldReturnNullIfNotFound()
        {
            const string path = @"C:\something";
            var actual = ChefRunner.FindChefInstallationDirectory(path, s => false);

            actual.Should().BeNull("because the file doesn't exist anywhere on the path");
        }

        [Fact]
        public void RubyExecutable_ShouldBeInEmbeddedDirectory()
        {
            var actual = ChefRunner.RubyExecutableWithin(ChefInstallPath);

            actual.Should().Be($@"{ChefInstallPath}\embedded\bin\ruby.exe");
        }

        [Fact]
        public void ChefClientLoaderWithin_ShouldBeInBinDirectory()
        {
            ChefRunner.ChefClientLoaderWithin(ChefInstallPath).Should().Be($@"{ChefInstallPath}\bin\chef-client");
        }
    }
}