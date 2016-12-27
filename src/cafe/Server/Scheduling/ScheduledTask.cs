using System;
using cafe.Shared;
using NodaTime;

namespace cafe.Server.Scheduling
{
    public class ScheduledTask : IScheduledTask
    {
        private readonly ScheduledTaskStatus _status;
        private readonly Action _action;
        private readonly IClock _clock;

        public ScheduledTask(string description, Action action, IClock clock)
        {
            _action = action;
            _clock = clock;
            _status = ScheduledTaskStatus.Create(description);
        }

        public void Run()
        {
            _status.StartTime = _clock.GetCurrentInstant().ToDateTimeUtc();
            _status.State = TaskState.Running;
            _action();
            _status.State = TaskState.Finished;
            _status.CompleteTime = _clock.GetCurrentInstant().ToDateTimeUtc();
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
    }
}