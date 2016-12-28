using System;
using cafe.Server.Scheduling;
using cafe.Shared;
using NLog;
using NLog.Fluent;
using NodaTime;

namespace cafe.Client
{
    public class SchedulerWaiter
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(SchedulerWaiter).FullName);

        private ISchedulerServer _schedulerServer;
        private ScheduledTaskStatus _originalStatus;
        private ScheduledTaskStatus _currentStatus;
        private readonly Func<ISchedulerServer> _schedulerServerProvider;
        private readonly IAutoResetEvent _autoResetEvent;
        private readonly ITimerFactory _timerFactory;
        private readonly TaskStatusPresenter _taskStatusPresenter;
        private readonly object _processorLocker = new object();


        public SchedulerWaiter(Func<ISchedulerServer> schedulerServerProvider, IAutoResetEvent autoResetEvent,
            ITimerFactory timerFactory, TaskStatusPresenter taskStatusPresenter)
        {
            _schedulerServerProvider = schedulerServerProvider;
            _autoResetEvent = autoResetEvent;
            _timerFactory = timerFactory;
            _taskStatusPresenter = taskStatusPresenter;
        }

        public ScheduledTaskStatus WaitForTaskToComplete(ScheduledTaskStatus status)
        {
            _originalStatus = status;
            _taskStatusPresenter.BeginPresenting(status);
            using (_timerFactory.ExecuteActionOnInterval(SchedulerCompletesStatus, Duration.FromSeconds(2)))
            {
                var maximumWait = TimeSpan.FromMinutes(15);
                Logger.Debug($"Waiting for {status} to complete");
                _autoResetEvent.WaitOne(maximumWait);
                Logger.Debug($"{status} has completed");
            }
            return _currentStatus;
        }

        private void SchedulerCompletesStatus()
        {
            lock (_processorLocker)
            {
                if (_schedulerServer == null)
                {
                    Logger.Debug("Creating rest api client for scheduler");
                    _schedulerServer = _schedulerServerProvider();
                }
                var taskId = _originalStatus.Id;
                try
                {
                    Log.Debug($"Fetching current status for task {taskId}");
                    _currentStatus = _schedulerServer.GetTaskStatus(taskId).Result;
                    Log.Debug($"Task {taskId} has status of {_currentStatus}");
                    _taskStatusPresenter.PresentAnyChangesTo(_currentStatus);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"While waiting for the task to finish, an exception was thrown: {ex.Message} Stack Trace: {ex.StackTrace}");
                    _currentStatus = _originalStatus.Copy();
                    _currentStatus.Result = Result.Failure(
                        "Lost connection to the server, and so couldn't finish processing this task");
                    _currentStatus.State = TaskState.Finished;
                    _currentStatus.CompleteTime = DateTime.Now;
                }
                var completed = _currentStatus != null && _currentStatus.State == TaskState.Finished;
                if (completed)
                {
                    Log.Info($"Task {taskId} has completed with status {_currentStatus} so moving forward");
                    _autoResetEvent.Set();
                }
            }
        }
    }
}