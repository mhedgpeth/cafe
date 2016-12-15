using Microsoft.Extensions.Logging;

namespace cafe
{
    public class Program
    {
        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<Program>();


        public static void Main(string[] args)
        {
            Logger.LogInformation("Starting cafe");
            if (args[0] == "chef")
            {
                if (args[1] == "run")
                {
                    Logger.LogInformation("Running chef");
                    var runner = CreateChefRunner();
                    runner.Run();
                    Logger.LogInformation("Finished running chef");
                }
                else if (args[1] == "version")
                {
                    var runner = CreateChefRunner();
                    var version = runner.RetrieveVersion();
                    Logger.LogInformation($"chef-client version: {version}");
                }
            }
        }

        private static ChefRunner CreateChefRunner()
        {
            var runner = new ChefRunner(() => new ChefProcess(() => new ProcessWrapper()));
            return runner;
        }
    }
}