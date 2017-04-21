namespace cafe.Chef
{
    public interface IRunChefPolicy
    {
        void PrepareEnvironmentForChefRun();
        string[] ArgumentsForChefRun();
    }
}