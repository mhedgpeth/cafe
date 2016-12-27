using System;
using NodaTime;

namespace cafe.Shared
{
    public class ScheduledTaskStatus
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public TaskState State { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? CompleteTime { get; set; }

        public static ScheduledTaskStatus Create(string description)
        {
            return new ScheduledTaskStatus
            {
                Id = Guid.NewGuid(),
                Description = description,
                State = TaskState.NotRun
            };
        }

        public override string ToString()
        {
            return $"Task {Description} ({State}) - Id: {Id}";
        }

        public ScheduledTaskStatus Copy()
        {
            // making a copy to defend from outsiders changing the state
            return new ScheduledTaskStatus()
            {
                Id = Id,
                Description = Description,
                State = State,
                StartTime = StartTime,
                CompleteTime = CompleteTime
            };
        }

        protected bool Equals(ScheduledTaskStatus other)
        {
            return Id.Equals(other.Id) && string.Equals(Description, other.Description) && State == other.State &&
                   StartTime.Equals(other.StartTime) && CompleteTime.Equals(other.CompleteTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ScheduledTaskStatus) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) State;
                hashCode = (hashCode * 397) ^ StartTime.GetHashCode();
                hashCode = (hashCode * 397) ^ CompleteTime.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ScheduledTaskStatus left, ScheduledTaskStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScheduledTaskStatus left, ScheduledTaskStatus right)
        {
            return !Equals(left, right);
        }
    }
}