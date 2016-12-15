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
                    var runner = new ChefRunner();
                    runner.Run();
                    Logger.LogInformation("Finished running chef");
                }
            }
        }
    }
}