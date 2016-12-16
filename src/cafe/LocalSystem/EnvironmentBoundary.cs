using Microsoft.Extensions.Logging;

namespace cafe.LocalSystem
{
    public class EnvironmentBoundary : IEnvironment
    {
        private static ILogger Logger { get; } =
          ApplicationLogging.CreateLogger<EnvironmentBoundary>();

        public string GetEnvironmentVariable(string key)
        {
            var value = System.Environment.GetEnvironmentVariable(key);
            Logger.LogDebug($"Retrieved environment variable {key} with value: {value}");
            return value;
        }
    }
}