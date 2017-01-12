namespace cafe.Chef
{
    public class NullBootstrapper : IChefBootstrapper
    {
        public void PrepareEnvironmentForChefRun()
        {
        }

        public string[] ArgumentsForChefRun()
        {
            return new string[0];
        }

        public override string ToString()
        {
            return "normally";
        }
    }
}