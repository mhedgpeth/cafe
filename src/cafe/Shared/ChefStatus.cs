using System;

namespace cafe.Shared
{
    public class ChefStatus : ProductStatus
    {
        public bool IsRunning { get; set; }
        public TimeSpan? Interval { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime? ExpectedNextRun { get; set; }

        public override string ToString()
        {
            var status = IsRunning ? "running" : "paused";
            var onDemand = IsRunning ? " on demand" : string.Empty;
            var every = Interval.HasValue ? $" every {((int) Interval.Value.TotalSeconds)} seconds" : onDemand;
            return $"Chef is {status}{every}";
        }
    }
}