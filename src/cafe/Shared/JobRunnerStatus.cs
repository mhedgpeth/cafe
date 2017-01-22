namespace cafe.Shared
{
    public class JobRunnerStatus
    {
        public JobRunStatus[] QueuedTasks { get; set; } = new JobRunStatus[0];
        public JobRunStatus[] FinishedTasks { get; set; } = new JobRunStatus[0];

        public override string ToString()
        {
            return $"Server has {QueuedTasks.Length} queued tasks and {FinishedTasks.Length} finished tasks";
        }
    }
}