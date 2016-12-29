namespace cafe.LocalSystem
{
    public interface IEnvironment
    {
        string GetEnvironmentVariable(string key);
        void SetSystemEnvironmentVariable(string key, string value);
    }
}