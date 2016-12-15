namespace cafe
{
    public interface IEnvironment
    {
        string GetEnvironmentVariable(string key);
    }

    public class Environment : IEnvironment
    {
        public string GetEnvironmentVariable(string key)
        {
            return System.Environment.GetEnvironmentVariable(key);
        }
    }
}