using cafe.Chef;
using FluentAssertions;
using Xunit;

namespace cafe.Test.Chef
{
    public class CafeDownloadUrlResolverTest
    {
        [Fact]
        public void DownloadUrlVersionFor_ShouldResolveGithubVersionForWin10Beta()
        {
            new CafeDownloadUrlResolver("win10").DownloadUriFor("0.5.4")
                .Should()
                .Be("https://github.com/mhedgpeth/cafe/releases/download/0.5.4-beta/cafe-win10-x64-0.5.4.0.zip");
        }

        [Fact]
        public void DownloadUrlVersionFor_ShouldResolveGithubVersionForWin7Beta()
        {
            new CafeDownloadUrlResolver("win7").DownloadUriFor("0.5.4")
                .Should()
                .Be("https://github.com/mhedgpeth/cafe/releases/download/0.5.4-beta/cafe-win7-x64-0.5.4.0.zip");
        }

        [Fact]
        public void FilenameFor_ShouldResolveWindows10File()
        {
            new CafeDownloadUrlResolver("win10").FilenameFor("0.5.4").Should().Be("cafe-win10-x64-0.5.4.0.zip");
        }

        [Fact]
        public void FilenameFor_ShouldResolveWindows7File()
        {
            new CafeDownloadUrlResolver("win7").FilenameFor("0.5.4").Should().Be("cafe-win7-x64-0.5.4.0.zip");

        }

        [Fact]
        public void DownloadUrlVersionFor_ShouldNotIncludeBetaInGithubVersionAfterVersion1()
        {
            new CafeDownloadUrlResolver("win10").DownloadUriFor("1.0.0")
                .Should()
                .Be("https://github.com/mhedgpeth/cafe/releases/download/1.0.0/cafe-win10-x64-1.0.0.0.zip");

        }

    }
}