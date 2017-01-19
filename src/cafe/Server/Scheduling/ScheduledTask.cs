using System;
using cafe.Shared;
using NLog;
using NodaTime;

namespace cafe.Server.Scheduling
{
    public class ScheduledTask : IScheduledTask, IMessagePresenter
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ScheduledTask).FullName);

        private ScheduledTaskStatus _status;
        private readonly Func<IMessagePresenter, Result> _action;
        private readonly RecurringTask _parentTask;
        private readonly IClock _clock;

        public ScheduledTask(string description, Func<IMessagePresenter, Result> action, RecurringTask parentTask,
            IClock clock)
        {
            _action = action;
            _parentTask = parentTask;
            _clock = clock;
            _status = ScheduledTaskStatus.Create(description);
        }

        public RecurringTask RecurringTask => _parentTask;

        public void Run()
        {
            _status = _status.ToRunningState(_clock.GetCurrentInstant().ToDateTimeUtc());
            ShowMessage($"Task {_status.Description} ({_status.Id}) started at {_status.StartTime}");
            var result = IsParentTaskPaused()
                ? Result.Inconclusive($"Task will not run because {_parentTask} is paused")
                : RunCore();
            _status = _status.ToFinishedState(result, _clock.GetCurrentInstant().ToDateTimeUtc());
            ShowMessage(
                $"Task {_status.Description} ({_status.Id}) completed at {_status.CompleteTime} with result: {_status.Result}");
        }

        private bool IsParentTaskPaused()
        {
            return _parentTask != null && !_parentTask.IsRunning;
        }

        private Result RunCore()
        {
            Result result;
            try
            {
                result = _action(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,
                    $"An unexpected error occurred while runnnig {_status.Description} ({_status.Id}): {ex.Message}");
                result = Result.Failure($"An unexpected error occurred: {ex.Message}");
            }
            return result;
        }

        public TaskState CurrentState => _status.State;
        public Guid Id => _status.Id;
        public DateTime? StartTime => _status.StartTime;
        public DateTime? CompleteTime => _status.CompleteTime;

        public override string ToString()
        {
            return _status.ToString();
        }

        public ScheduledTaskStatus ToTaskStatus()
        {
            return _status.Copy();
        }

        public void ShowMessage(string message)
        {
            Logger.Info(message);
            _status.CurrentMessage = message;
        }
    }
}