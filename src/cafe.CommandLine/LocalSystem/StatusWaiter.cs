using System;
using NLog;
using NLog.Fluent;
using NodaTime;

namespace cafe.CommandLine.LocalSystem
{
    public abstract class StatusWaiter<T>
    {
        private static readonly Logger Logger = LogManager.GetLogger("cafe.CommandLine.LocalSystem.StatusWaiter");

        private readonly string _taskDescription;
        private readonly IAutoResetEvent _autoResetEvent;
        private readonly ITimerFactory _timerFactory;
        private readonly object _processorLocker = new object();

        private T _currentStatus;

        protected StatusWaiter(string taskDescription, IAutoResetEvent autoResetEvent, ITimerFactory timerFactory)
        {
            _taskDescription = taskDescription;
            _autoResetEvent = autoResetEvent;
            _timerFactory = timerFactory;
        }

        protected T Wait()
        {
            Logger.Debug($"Waiting for {_taskDescription} to complete");
            using (_timerFactory.ExecuteActionOnInterval(DetermineIfTaskIsComplete, Duration.FromSeconds(2)))
            {
                var maximumWait = TimeSpan.FromMinutes(15);
                _autoResetEvent.WaitOne(maximumWait);
                Logger.Debug($"{_taskDescription} has completed");
            }
            return _currentStatus;
        }

        private void DetermineIfTaskIsComplete()
        {
            lock (_processorLocker)
            {
                _currentStatus = RetrieveCurrentStatus();
                if (IsCurrentStatusCompleted(_currentStatus))
                {
                    Logger.Info($"Task {_taskDescription} has completed with status {_currentStatus} so moving forward");
                    _autoResetEvent.Set();
                }
            }
        }

        protected abstract bool IsCurrentStatusCompleted(T currentStatus);

        protected abstract T RetrieveCurrentStatus();
    }
}