namespace cafe.Shared
{
    public class ServerStatus
    {
        public JobRunStatus[] QueuedTasks { get; set; } = new JobRunStatus[0];
        public JobRunStatus[] FinishedTasks { get; set; } = new JobRunStatus[0];
        public ChefStatus ChefStatus { get; set; }

        public override string ToString()
        {
            return $"Server has 0 queued tasks and 0 finished tasks and {ChefStatus}";
        }
    }
}