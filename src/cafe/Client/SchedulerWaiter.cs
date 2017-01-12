using System;
using cafe.Server.Scheduling;
using cafe.Shared;
using NLog;
using NLog.Fluent;

namespace cafe.Client
{
    public interface ISchedulerWaiter
    {
        ScheduledTaskStatus WaitForTaskToComplete(ScheduledTaskStatus status);
    }

    public class SchedulerWaiter : StatusWaiter<ScheduledTaskStatus>, ISchedulerWaiter
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(SchedulerWaiter).FullName);

        private ISchedulerServer _schedulerServer;
        private ScheduledTaskStatus _originalStatus;
        private readonly Func<ISchedulerServer> _schedulerServerProvider;
        private readonly TaskStatusPresenter _taskStatusPresenter;


        public SchedulerWaiter(Func<ISchedulerServer> schedulerServerProvider, IAutoResetEvent autoResetEvent,
            ITimerFactory timerFactory, TaskStatusPresenter taskStatusPresenter)
            : base("status", autoResetEvent, timerFactory)
        {
            _schedulerServerProvider = schedulerServerProvider;
            _taskStatusPresenter = taskStatusPresenter;
        }

        public ScheduledTaskStatus WaitForTaskToComplete(ScheduledTaskStatus status)
        {
            _originalStatus = status;
            _taskStatusPresenter.BeginPresenting(status);
            return Wait();
        }

        protected override bool IsCurrentStatusCompleted(ScheduledTaskStatus currentStatus)
        {
            return currentStatus != null && currentStatus.State == TaskState.Finished;
        }

        protected override ScheduledTaskStatus RetrieveCurrentStatus()
        {
            if (_schedulerServer == null)
            {
                Logger.Debug("Creating rest api client for scheduler");
                _schedulerServer = _schedulerServerProvider();
            }
            var taskId = _originalStatus.Id;
            ScheduledTaskStatus currentStatus;
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
                currentStatus.State = TaskState.Finished;
                currentStatus.CompleteTime = DateTime.Now;
            }
            return currentStatus;
        }
    }
}