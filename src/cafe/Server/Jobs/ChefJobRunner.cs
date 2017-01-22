using System;
using cafe.Shared;
using NLog;

namespace cafe.Server.Jobs
{
    public class ChefJobRunner : ProductJobRunner<ChefStatus>
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefJobRunner).FullName);

        private readonly RunChefJob _runChefJob;

        public ChefJobRunner(JobRunner runner, DownloadJob downloadJob, InstallJob installJob,
            RunChefJob runChefJob) : base(runner, downloadJob, installJob)
        {
            ListenForJobsToBeReady(runChefJob);
            _runChefJob = runChefJob;
        }

        public RunChefJob RunChefJob => _runChefJob;

        public override ChefStatus ToStatus()
        {
            return new ChefStatus
            {
                IsRunning = RunChefJob.IsRunning,
                ExpectedNextRun = RunChefJob.RunPolicy.ExpectedNextRun?.ToDateTimeUtc(),
                Interval = RunChefJob.RunPolicy.Interval?.ToTimeSpan(),
                LastRun = RunChefJob.LastRun?.Start?.ToDateTimeUtc(),
                Version = InstallJob.CurrentVersion?.ToString()
            };
        }
    }
}