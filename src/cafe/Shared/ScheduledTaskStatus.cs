using System;

namespace cafe.Shared
{
    public class ScheduledTaskStatus
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public TaskState State { get; set; }

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
                State = State
            };
        }

        protected bool Equals(ScheduledTaskStatus other)
        {
            return Id.Equals(other.Id) && string.Equals(Description, other.Description) && State == other.State;
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
            return Id.GetHashCode();
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