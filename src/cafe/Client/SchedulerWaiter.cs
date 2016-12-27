using System;
using System.Threading;
using cafe.Server.Scheduling;
using cafe.Shared;
using NLog;

namespace cafe.Client
{
    public class SchedulerWaiter
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Scheduler).FullName);

        private readonly ClientFactory _clientFactory;
        private ISchedulerServer _schedulerServer;
        private ScheduledTaskStatus _originalStatus;
        private ScheduledTaskStatus _currentStatus;
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        public SchedulerWaiter(ClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public ScheduledTaskStatus WaitForTaskToComplete(ScheduledTaskStatus status)
        {
            _originalStatus = status;
            var checkInterval = TimeSpan.FromSeconds(2);
            using (new Timer(state => SchedulerCompletesStatus(), new object(), TimeSpan.Zero,
                checkInterval))
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
                _schedulerServer = _clientFactory.RestClientForSchedulerServer();
            }
            _currentStatus = _schedulerServer.GetTaskStatus(_originalStatus.Id).Result;
            var completed = _currentStatus != null && _currentStatus.State == TaskState.Finished;
            if (completed)
            {
                _autoResetEvent.Set();
            }
        }
    }
}