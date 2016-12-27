using System;
using cafe.Shared;
using NLog;
using NodaTime;

namespace cafe.Server.Scheduling
{
    public class ScheduledTask : IScheduledTask, IMessagePresenter
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ScheduledTask).FullName);

        private readonly ScheduledTaskStatus _status;
        private readonly Func<IMessagePresenter, Result> _action;
        private readonly IClock _clock;

        public ScheduledTask(string description, Func<IMessagePresenter, Result> action, IClock clock)
        {
            _action = action;
            _clock = clock;
            _status = ScheduledTaskStatus.Create(description);
        }

        public void Run()
        {
            _status.StartTime = _clock.GetCurrentInstant().ToDateTimeUtc();
            _status.State = TaskState.Running;
            ShowMessage($"Task {_status.Description} ({_status.Id}) started at {_status.StartTime}");
            try
            {
                _status.Result = _action(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"An unexpected error occurred while runnnig {_status.Description} ({_status.Id}): {ex.Message}");
                _status.Result = Result.Failure($"An unexpected error occurred: {ex.Message}");
            }
            _status.State = TaskState.Finished;
            _status.CompleteTime = _clock.GetCurrentInstant().ToDateTimeUtc();
            ShowMessage($"Task {_status.Description} ({_status.Id}) completed at {_status.CompleteTime} with result: {_status.Result}");
        }

        public TaskState CurrentState => _status.State;
        public Guid Id { get; } = Guid.NewGuid();
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