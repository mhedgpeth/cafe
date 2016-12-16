using cafe.Chef;
using cafe.CommandLine;
using cafe.LocalSystem;
using cafe.Options;
using Microsoft.Extensions.Logging;

namespace cafe
{
    public class Program
    {
        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<Program>();


        public static void Main(string[] args)
        {
            Logger.LogInformation($"Running cafe {System.Reflection.Assembly.GetEntryAssembly().GetName().Version} with arguments {string.Join(" ", args)}");
            var chefRunner = CreateChefRunner();
            var runner = new Runner(
                new RunChefOption(chefRunner),
                new ShowChefVersionOption(chefRunner),
                new DownloadChefOption(new ChefDownloader(new FileDownloader(), new FileSystem())));
            runner.Run(args);
            Logger.LogDebug("Finishing cafe run");
        }

        private static ChefRunner CreateChefRunner()
        {
            return new ChefRunner(() => new ChefProcess(() => new ProcessBoundary(), new FileSystem(), new EnvironmentBoundary()));
        }
    }
}