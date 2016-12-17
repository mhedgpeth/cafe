using System;
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
            var runner = CreateRunner(args);
            runner.Run(args);
            Logger.LogDebug("Finishing cafe run");
        }

        public static Runner CreateRunner(string[] args)
        {
            Logger.LogInformation(
                $"Running cafe {System.Reflection.Assembly.GetEntryAssembly().GetName().Version} with arguments {string.Join(" ", args)}");
            Logger.LogDebug("Creating chef runner");
            var chefRunner = CreateChefRunner();
            Logger.LogDebug("Creating runner");
            var runner = new Runner(
                new RunChefOption(chefRunner),
                new ShowChefVersionOption(chefRunner),
                new DownloadChefOption(new ChefDownloader(new FileDownloader(),
                    new FileSystem(new EnvironmentBoundary(), new FileSystemCommandsBoundary()))),
                new UpgradeChefOption());
            Logger.LogDebug("Running application");
            return runner;
        }

        private static ChefRunner CreateChefRunner()
        {
            return
                new ChefRunner(
                    () =>
                        new ChefProcess(new ProcessExecutor(() => new ProcessBoundary()),
                            new FileSystem(new EnvironmentBoundary(), new FileSystemCommandsBoundary())));
        }
    }
}