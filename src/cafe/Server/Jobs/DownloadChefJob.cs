using cafe.Chef;
using cafe.Shared;
using NodaTime;

namespace cafe.Server.Jobs
{
    public class DownloadChefJob : Job
    {

        private readonly IDownloader _downloader;
        private readonly IClock _clock;

        public DownloadChefJob(IDownloader downloader, IClock clock)
        {
            _downloader = downloader;
            _clock = clock;
        }

        public ScheduledTaskStatus Download(string version)
        {
            return OnRunReady(new JobRun($"Download Chef {version}", presenter => _downloader.Download(version, presenter), _clock));
        }
    }
}