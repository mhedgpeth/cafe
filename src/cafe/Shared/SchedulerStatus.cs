namespace cafe.Shared
{
    public class SchedulerStatus
    {
        public ScheduledTaskStatus[] QueuedTasks { get; set; }
        public ScheduledTaskStatus[] FinishedTasks { get; set; }
        public ChefStatus ChefStatus { get; set; }
    }
}