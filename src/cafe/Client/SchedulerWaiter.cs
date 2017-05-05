using System;
using cafe.CommandLine;
using cafe.CommandLine.LocalSystem;
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
        private readonly IMessagePresenter _messagePresenter;
        private int _previousMessageIndex = 0;


        public SchedulerWaiter(Func<IJobServer> jobServerProvider, IAutoResetEvent autoResetEvent,
            ITimerFactory timerFactory, IMessagePresenter messagePresenter)
            : base("status", autoResetEvent, timerFactory)
        {
            _jobServerProvider = jobServerProvider;
            _messagePresenter = messagePresenter;
        }

        public JobRunStatus WaitForTaskToComplete(JobRunStatus status)
        {
            _originalStatus = status;
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
                Logger.Debug($"Fetching current status for task {taskId} with previous index of {_previousMessageIndex}");
                currentStatus = _jobServer.GetJobRunStatus(taskId, _previousMessageIndex).Result;
                _previousMessageIndex = currentStatus.CurrentMessageIndex;
                Logger.Debug($"Task {taskId} has status of {currentStatus}");
                Logger.Debug($"Showing all {currentStatus.Messages.Length} messages");
                foreach (var statusMessage in currentStatus.Messages)
                {
                    Logger.Debug($"Message: {statusMessage}");
                    _messagePresenter.ShowMessage(statusMessage);
                }
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