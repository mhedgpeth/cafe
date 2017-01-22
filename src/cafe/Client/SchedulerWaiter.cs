using System;
using cafe.Server.Scheduling;
using cafe.Shared;
using NLog;
using NLog.Fluent;

namespace cafe.Client
{
    public interface ISchedulerWaiter
    {
        JobRunStatus WaitForTaskToComplete(JobRunStatus status);
    }

    public class SchedulerWaiter : StatusWaiter<JobRunStatus>, ISchedulerWaiter
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(SchedulerWaiter).FullName);

        private IJobServer _jobServer;
        private JobRunStatus _originalStatus;
        private readonly Func<IJobServer> _jobServerProvider;
        private readonly JobRunStatusPresenter _jobRunStatusPresenter;


        public SchedulerWaiter(Func<IJobServer> jobServerProvider, IAutoResetEvent autoResetEvent,
            ITimerFactory timerFactory, JobRunStatusPresenter jobRunStatusPresenter)
            : base("status", autoResetEvent, timerFactory)
        {
            _jobServerProvider = jobServerProvider;
            _jobRunStatusPresenter = jobRunStatusPresenter;
        }

        public JobRunStatus WaitForTaskToComplete(JobRunStatus status)
        {
            _originalStatus = status;
            _jobRunStatusPresenter.BeginPresenting(status);
            return Wait();
        }

        protected override bool IsCurrentStatusCompleted(JobRunStatus currentStatus)
        {
            return currentStatus != null && currentStatus.State == JobRunState.Finished;
        }

        protected override JobRunStatus RetrieveCurrentStatus()
        {
            if (_jobServer == null)
            {
                Logger.Debug("Creating rest api client for scheduler");
                _jobServer = _jobServerProvider();
            }
            var taskId = _originalStatus.Id;
            JobRunStatus currentStatus;
            try
            {
                Log.Debug($"Fetching current status for task {taskId}");
                currentStatus = _jobServer.GetJobRunStatus(taskId).Result;
                Log.Debug($"Task {taskId} has status of {currentStatus}");
                _jobRunStatusPresenter.PresentAnyChangesTo(currentStatus);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,
                    $"While waiting for the task to finish, an exception was thrown: {ex.Message} Stack Trace: {ex.StackTrace}");
                currentStatus = _originalStatus.Copy();
                currentStatus.Result = Result.Failure(
                    "Lost connection to the server, and so couldn't finish processing this task");
                currentStatus.State = JobRunState.Finished;
                currentStatus.FinishTime = DateTime.Now;
            }
            return currentStatus;
        }
    }
}