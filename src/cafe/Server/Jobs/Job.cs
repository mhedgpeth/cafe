using System;
using cafe.Shared;
using NLog;

namespace cafe.Server.Jobs
{
    public abstract class Job
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Job).FullName);

        public event EventHandler<JobRun> RunReady;

        private JobRun _lastRun;

        public JobRun LastRun => _lastRun;

        protected virtual ScheduledTaskStatus OnRunReady(JobRun run)
        {
            Logger.Debug($"Firing run ready for {run}");
            _lastRun = run;
            RunReady?.Invoke(this, run);
            return run.ToStatus();
        }
    }
}