namespace cafe.Server.Scheduling
{
    public class SchedulerStatus
    {
        public bool IsRunning { get; set; }
        public int QueuedTasks { get; set; }
    }
}