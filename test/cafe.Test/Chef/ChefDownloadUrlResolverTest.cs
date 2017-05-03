using cafe.Chef;
using cafe.CommandLine.LocalSystem;
using cafe.LocalSystem;
using FluentAssertions;
using Moq;
using Xunit;

namespace cafe.Test.Chef
{
    public class ChefDownloadUrlResolverTest
    {
        private const string Product = "chef";
        private const string Prefix = "chef-client";

        [Fact]
        public void DownloadUriFor_ShouldProduceDownloadLinkForLatestVersion()
        {
            CreateResolver().DownloadUriFor(LatestVersion).AbsoluteUri
                .Should()
                .Be("https://packages.chef.io/files/stable/chef/12.17.44/windows/2012r2/chef-client-12.17.44-1-x64.msi");
        }

        [Fact]
        public void DownloadUriFor_ShouldProduceDownloadLinkForPreviousVersion()
        {
            CreateResolver().DownloadUriFor(PreviousVersion).AbsoluteUri
                .Should()
                .Be("https://packages.chef.io/files/stable/chef/12.14.77/windows/2012r2/chef-client-12.14.77-1-x64.msi");
        }

        private static ChefDownloadUrlResolver CreateResolver()
        {
            return new ChefDownloadUrlResolver(Product, Prefix, "2012r2");
        }

        private const string LatestVersion = "12.17.44";

        [Fact]
        public void FilenameFor_ShouldProduceFilenameForLatestVersion()
        {
            CreateResolver().FilenameFor(LatestVersion)
                .Should()
                .Be("chef-client-12.17.44-1-x64.msi");
        }

        [Fact]
        public void FilenameFor_ShouldProduceFilenameForPreviousVersion()
        {
            CreateResolver().FilenameFor(PreviousVersion)
                .Should()
                .Be("chef-client-12.14.77-1-x64.msi");
        }

        private const string PreviousVersion = "12.14.77";

        [Fact]
        public void Download_ShouldEnsureStagingDirectoryExists()
        {
            var fileDownloader = new FakeFileDownloader();
            var fileSystem = new Mock<IFileSystem>();
            var chefDownloader = new Downloader(fileDownloader, fileSystem.Object, Product, CreateResolver());
            var shareDirectory = Downloader.StagingDirectory;
            chefDownloader.Download(PreviousVersion, new FakeMessagePresenter());

            fileSystem.Verify(f => f.EnsureDirectoryExists(shareDirectory));
        }
    }
}