namespace cafe.Server.Scheduling
{
    public interface IScheduledTask
    {
        bool IsFinishedRunning { get; set; }
        bool IsRunning { get; set; }
        void Run();
    }
}