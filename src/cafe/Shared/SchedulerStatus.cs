namespace cafe.Shared
{
    public class SchedulerStatus
    {
        public bool IsRunning { get; set; }
        public ScheduledTaskStatus[] QueuedTasks { get; set; }
        public ScheduledTaskStatus[] FinishedTasks { get; set; }
    }
}