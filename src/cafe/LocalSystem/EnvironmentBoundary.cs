using Microsoft.Extensions.Logging;
using NLog;

namespace cafe.LocalSystem
{
    public class EnvironmentBoundary : IEnvironment
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(EnvironmentBoundary).FullName);

        public string GetEnvironmentVariable(string key)
        {
            var value = System.Environment.GetEnvironmentVariable(key);
            Logger.Debug($"Retrieved environment variable {key} with value: {value}");
            return value;
        }
    }
}