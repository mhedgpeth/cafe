using System;
using cafe.CommandLine;
using NodaTime;

namespace cafe.Shared
{
    public class JobRunStatus
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public JobRunState State { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public Result Result { get; set; }
        public string CurrentMessage { get; set; }
        public bool IsNotRun => State == JobRunState.NotRun;
        public bool IsRunning => State == JobRunState.Running;

        public TimeSpan? Duration
        {
            get
            {
                if (StartTime.HasValue && FinishTime.HasValue) return FinishTime.Value.Subtract(StartTime.Value);
                if (StartTime.HasValue) return SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc().Subtract(StartTime.Value);
                return null;
            }
        }

        public static JobRunStatus Create(string description)
        {
            return new JobRunStatus
            {
                Id = Guid.NewGuid(),
                Description = description,
                State = JobRunState.NotRun
            };
        }

        public override string ToString()
        {
            var firstPart = $"Task {Description} ({Id}) - ";
            if (IsNotRun)
            {
                return $"{firstPart}Not yet run";
            }
            if (IsRunning)
            {
                return $"{firstPart}Running for {(int) Duration.Value.TotalSeconds} seconds";
            }
            return $"{firstPart}{Result}";
        }

        public JobRunStatus Copy()
        {
            // making a copy to defend from outsiders changing the state
            return new JobRunStatus()
            {
                Id = Id,
                Description = Description,
                State = State,
                StartTime = StartTime,
                FinishTime = FinishTime,
                Result = Result,
                CurrentMessage = CurrentMessage
            };
        }

        protected bool Equals(JobRunStatus other)
        {
            return Id.Equals(other.Id) && string.Equals(Description, other.Description) && State == other.State &&
                   StartTime.Equals(other.StartTime) && FinishTime.Equals(other.FinishTime) &&
                   Equals(Result, other.Result) && string.Equals(CurrentMessage, other.CurrentMessage);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((JobRunStatus) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) State;
                hashCode = (hashCode * 397) ^ StartTime.GetHashCode();
                hashCode = (hashCode * 397) ^ FinishTime.GetHashCode();
                hashCode = (hashCode * 397) ^ (Result != null ? Result.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CurrentMessage != null ? CurrentMessage.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(JobRunStatus left, JobRunStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(JobRunStatus left, JobRunStatus right)
        {
            return !Equals(left, right);
        }

        public JobRunStatus ToRunningState(DateTime startTime)
        {
            var copy = Copy();
            copy.StartTime = startTime;
            copy.State = JobRunState.Running;
            return copy;
        }

        public JobRunStatus ToFinishedState(Result result, DateTime completeTime)
        {
            var copy = Copy();
            copy.Result = result;
            copy.FinishTime = completeTime;
            copy.State = JobRunState.Finished;
            return copy;
        }

        public JobRunStatus WithDifferentMessage(string message)
        {
            var copy = Copy();
            copy.CurrentMessage = message;
            return copy;
        }
    }
}