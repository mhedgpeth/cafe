namespace cafe.Options.Server
{
    public enum ServiceStatus
    {
        Undetermined = 0,
        Stopped = 1,
        IsStarting = 2,
        IsStopping = 3,
        Running = 4,
        ContinuePending = 5,
        PausePending = 6,
        Paused = 7
    }
}