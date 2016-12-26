namespace cafe.Server
{
    public interface IScheduledTask
    {
        bool IsFinishedRunning { get; set; }
        bool IsRunning { get; set; }
        void Run();
    }
}