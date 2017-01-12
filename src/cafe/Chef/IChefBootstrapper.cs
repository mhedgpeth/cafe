namespace cafe.Chef
{
    public interface IChefBootstrapper
    {
        void PrepareEnvironmentForChefRun();
        string[] ArgumentsForChefRun();
    }
}