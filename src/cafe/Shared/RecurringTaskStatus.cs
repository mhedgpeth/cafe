using System;

namespace cafe.Shared
{
    public class RecurringTaskStatus
    {
        public string Name { get; set; }
        public bool IsRunning { get; set; }
        public DateTime Created { get; set; }
        public TimeSpan Interval { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime ExpectedNextRun { get; set; }

        protected bool Equals(RecurringTaskStatus other)
        {
            return string.Equals(Name, other.Name) && IsRunning == other.IsRunning && Created.Equals(other.Created) &&
                   Interval.Equals(other.Interval) && LastRun.Equals(other.LastRun) &&
                   ExpectedNextRun.Equals(other.ExpectedNextRun);
        }

        public override string ToString()
        {
            return IsRunning
                ? $"Task {Name} running every {(int) Interval.TotalSeconds} seconds"
                : $"Task {Name} is paused";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RecurringTaskStatus) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsRunning.GetHashCode();
                hashCode = (hashCode * 397) ^ Created.GetHashCode();
                hashCode = (hashCode * 397) ^ Interval.GetHashCode();
                hashCode = (hashCode * 397) ^ LastRun.GetHashCode();
                hashCode = (hashCode * 397) ^ ExpectedNextRun.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(RecurringTaskStatus left, RecurringTaskStatus right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RecurringTaskStatus left, RecurringTaskStatus right)
        {
            return !Equals(left, right);
        }
    }
}