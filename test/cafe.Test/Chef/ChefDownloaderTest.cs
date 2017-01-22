using cafe.Chef;
using cafe.LocalSystem;
using FluentAssertions;
using Moq;
using Xunit;

namespace cafe.Test.Chef
{
    public class ChefDownloaderTest
    {
        private const string Product = "chef";
        private const string Prefix = "chef-client";

        [Fact]
        public void DownloadUriFor_ShouldProduceDownloadLinkForLatestVersion()
        {
            Downloader.DownloadUriFor(LatestVersion, Product, Prefix, "2012r2").AbsoluteUri
                .Should()
                .Be("https://packages.chef.io/files/stable/chef/12.17.44/windows/2012r2/chef-client-12.17.44-1-x64.msi");
        }

        [Fact]
        public void DownloadUriFor_ShouldProduceDownloadLinkForPreviousVersion()
        {
            Downloader.DownloadUriFor(PreviousVersion, Product, Prefix, "2012r2").AbsoluteUri
                .Should()
                .Be("https://packages.chef.io/files/stable/chef/12.14.77/windows/2012r2/chef-client-12.14.77-1-x64.msi");
        }

        private const string LatestVersion = "12.17.44";

        [Fact]
        public void FilenameFor_ShouldProduceFilenameForLatestVersion()
        {
            Downloader.FilenameFor(LatestVersion, Prefix)
                .Should()
                .Be("chef-client-12.17.44-1-x64.msi");
        }

        [Fact]
        public void FilenameFor_ShouldProduceFilenameForPreviousVersion()
        {
            Downloader.FilenameFor(PreviousVersion, Prefix)
                .Should()
                .Be("chef-client-12.14.77-1-x64.msi");
        }

        private const string PreviousVersion = "12.14.77";

        [Fact]
        public void Download_ShouldEnsureStagingDirectoryExists()
        {
            var fileDownloader = new FakeFileDownloader();
            var fileSystem = new Mock<IFileSystem>();
            var chefDownloader = new Downloader(fileDownloader, fileSystem.Object, Product, Prefix, "2012r2");
            var shareDirectory = Downloader.StagingDirectory;
            chefDownloader.Download(PreviousVersion, new FakeMessagePresenter());

            fileSystem.Verify(f => f.EnsureDirectoryExists(shareDirectory));
        }
    }
}