using System;

namespace cafe.Shared
{
    public class ChefStatus
    {
        public string Version { get; set; }
        public bool IsRunning { get; set; }
        public TimeSpan? Interval { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime? ExpectedNextRun { get; set; }

    }
}