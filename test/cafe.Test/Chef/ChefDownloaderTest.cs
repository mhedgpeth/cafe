using System;
using cafe.Chef;
using cafe.LocalSystem;
using cafe.Server.Scheduling;
using cafe.Shared;
using FluentAssertions;
using Moq;
using Xunit;

namespace cafe.Test.Chef
{
    public class ChefDownloaderTest
    {
        [Fact]
        public void DownloadUriFor_ShouldProduceDownloadLinkForLatestVersion()
        {
            ChefDownloader.DownloadUriFor(LatestVersion).AbsoluteUri
                .Should()
                .Be("https://packages.chef.io/files/stable/chef/12.17.44/windows/2012r2/chef-client-12.17.44-1-x64.msi");
        }

        [Fact]
        public void DownloadUriFor_ShouldProduceDownloadLinkForPreviousVersion()
        {
            ChefDownloader.DownloadUriFor(PreviousVersion).AbsoluteUri
                .Should()
                .Be("https://packages.chef.io/files/stable/chef/12.14.77/windows/2012r2/chef-client-12.14.77-1-x64.msi");
        }

        private const string LatestVersion = "12.17.44";

        [Fact]
        public void FilenameFor_ShouldProduceFilenameForLatestVersion()
        {
            ChefDownloader.FilenameFor(LatestVersion)
                .Should()
                .Be("chef-client-12.17.44-1-x64.msi");
        }

        [Fact]
        public void FilenameFor_ShouldProduceFilenameForPreviousVersion()
        {
            ChefDownloader.FilenameFor(PreviousVersion)
                .Should()
                .Be("chef-client-12.14.77-1-x64.msi");
        }

        private const string PreviousVersion = "12.14.77";

        [Fact]
        public void Download_ShouldEnsureStagingDirectoryExists()
        {
            var fileDownloader = new FakeFileDownloader();
            var fileSystem = new Mock<IFileSystem>();
            var chefDownloader = new ChefDownloader(fileDownloader, fileSystem.Object);
            var shareDirectory = ChefDownloader.StagingDirectory;
            chefDownloader.Download(PreviousVersion, new FakeMessagePresenter());

            fileSystem.Verify(f => f.EnsureDirectoryExists(shareDirectory));
        }
    }

    public class FakeMessagePresenter : IMessagePresenter
    {
        public void ShowMessage(string message)
        {
            MessageShown = message;
            WasMessageShown = true;
        }

        public string MessageShown { get; set; }

        public void Clear()
        {
            WasMessageShown = false;
        }

        public bool WasMessageShown { get; private set; }
    }

    public class FakeFileDownloader : IFileDownloader
    {
        public Result Download(Uri downloadLink, string file)
        {
            return Result.Successful();
        }
    }
}