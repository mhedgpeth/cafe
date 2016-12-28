using System;

namespace cafe.Shared
{
    public class RecurringTaskStatus
    {
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public TimeSpan Interval { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime ExpectedNextRun { get; set; }
    }
}