using System;
using cafe.Server.Scheduling;
using cafe.Shared;
using NLog;
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

        public SchedulerWaiter(Func<ISchedulerServer> schedulerServerProvider, IAutoResetEvent autoResetEvent,
            ITimerFactory timerFactory)
        {
            _schedulerServerProvider = schedulerServerProvider;
            _autoResetEvent = autoResetEvent;
            _timerFactory = timerFactory;
        }

        public ScheduledTaskStatus WaitForTaskToComplete(ScheduledTaskStatus status)
        {
            _originalStatus = status;
            using (_timerFactory.ExecuteActionOnInterval(SchedulerCompletesStatus, Duration.FromSeconds(2)))
            {
                var maximumWait = TimeSpan.FromMinutes(15);
                _autoResetEvent.WaitOne(maximumWait);
            }
            return _currentStatus;
        }

        private void SchedulerCompletesStatus()
        {
            if (_schedulerServer == null)
            {
                _schedulerServer = _schedulerServerProvider();
            }
            try
            {
                _currentStatus = _schedulerServer.GetTaskStatus(_originalStatus.Id).Result;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "While waiting for the task to finish, an exception was thrown");
                _currentStatus = _originalStatus.Copy();
                _currentStatus.Result = Result.Failure(
                    "Lost connection to the server, and so couldn't finish processing this task");
                _currentStatus.State = TaskState.Finished;
                _currentStatus.CompleteTime = DateTime.Now;
            }
            var completed = _currentStatus != null && _currentStatus.State == TaskState.Finished;
            if (completed)
            {
                _autoResetEvent.Set();
            }
        }
    }
}