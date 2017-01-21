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

        private IChefServer _schedulerServer;
        private JobRunStatus _originalStatus;
        private readonly Func<IChefServer> _schedulerServerProvider;
        private readonly TaskStatusPresenter _taskStatusPresenter;


        public SchedulerWaiter(Func<IChefServer> schedulerServerProvider, IAutoResetEvent autoResetEvent,
            ITimerFactory timerFactory, TaskStatusPresenter taskStatusPresenter)
            : base("status", autoResetEvent, timerFactory)
        {
            _schedulerServerProvider = schedulerServerProvider;
            _taskStatusPresenter = taskStatusPresenter;
        }

        public JobRunStatus WaitForTaskToComplete(JobRunStatus status)
        {
            _originalStatus = status;
            _taskStatusPresenter.BeginPresenting(status);
            return Wait();
        }

        protected override bool IsCurrentStatusCompleted(JobRunStatus currentStatus)
        {
            return currentStatus != null && currentStatus.State == JobRunState.Finished;
        }

        protected override JobRunStatus RetrieveCurrentStatus()
        {
            if (_schedulerServer == null)
            {
                Logger.Debug("Creating rest api client for scheduler");
                _schedulerServer = _schedulerServerProvider();
            }
            var taskId = _originalStatus.Id;
            JobRunStatus currentStatus;
            try
            {
                Log.Debug($"Fetching current status for task {taskId}");
                currentStatus = _schedulerServer.GetTaskStatus(taskId).Result;
                Log.Debug($"Task {taskId} has status of {currentStatus}");
                _taskStatusPresenter.PresentAnyChangesTo(currentStatus);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,
                    $"While waiting for the task to finish, an exception was thrown: {ex.Message} Stack Trace: {ex.StackTrace}");
                currentStatus = _originalStatus.Copy();
                currentStatus.Result = Result.Failure(
                    "Lost connection to the server, and so couldn't finish processing this task");
                currentStatus.State = JobRunState.Finished;
                currentStatus.CompleteTime = DateTime.Now;
            }
            return currentStatus;
        }
    }
}