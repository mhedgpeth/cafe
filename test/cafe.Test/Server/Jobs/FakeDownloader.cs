using cafe.Chef;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Test.Server.Jobs
{
    public class FakeDownloader : IDownloader
    {
        public string Product => "Fake Product";

        public Result Download(string version, IMessagePresenter messagePresenter)
        {
            DownloadedVersion = version;
            return Result.Successful();
        }

        public string DownloadedVersion { get; private set; }
    }
}