using cafe.Shared;
using NLog;

namespace cafe.Server.Jobs
{
    public abstract class ProductJobRunner<T> where T : ProductStatus
    {
        private static readonly Logger Logger = LogManager.GetLogger("cafe.Server.Jobs.ProductJobRunner");

        private readonly JobRunner _runner;
        private readonly DownloadJob _downloadJob;
        private readonly InstallJob _installJob;

        protected ProductJobRunner(JobRunner runner, DownloadJob downloadJob, InstallJob installJob)
        {
            ListenForJobsToBeReady(downloadJob, installJob);
            _runner = runner;
            _downloadJob = downloadJob;
            _installJob = installJob;
        }

        protected void ListenForJobsToBeReady(params Job[] jobs)
        {
            foreach (var job in jobs)
            {
                job.RunReady += JobRunReady;
            }
        }

        public DownloadJob DownloadJob => _downloadJob;

        public InstallJob InstallJob => _installJob;


        private void JobRunReady(object sender, JobRun jobRun)
        {
            Logger.Debug($"Adding {jobRun} to the queue to run");
            _runner.Enqueue(jobRun);
        }

        public abstract T ToStatus();
    }
}