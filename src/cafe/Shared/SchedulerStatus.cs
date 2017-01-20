namespace cafe.Shared
{
    public class SchedulerStatus
    {
        public ScheduledTaskStatus[] QueuedTasks { get; set; }
        public ScheduledTaskStatus[] FinishedTasks { get; set; }
        public RecurringTaskStatus[] RecurringTasks { get; set; }
    }
}