using cafe.Chef;
using cafe.Shared;
using NodaTime;

namespace cafe.Server.Jobs
{
    public class DownloadJob : Job
    {
        private readonly IDownloader _downloader;
        private readonly IClock _clock;

        public DownloadJob(IDownloader downloader, IClock clock)
        {
            _downloader = downloader;
            _clock = clock;
        }

        public JobRunStatus Download(string version)
        {
            return OnRunReady(new JobRun($"Download {_downloader.Product} {version}", presenter => _downloader.Download(version, presenter), _clock));
        }
    }
}