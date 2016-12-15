using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace cafe
{
    public class ChefRunner
    {
        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<ChefRunner>();

        public void Run()
        {
            Logger.LogInformation("Running chef-client");
            var chefInstallDirectory = FindChefInstallationDirectory(Environment.GetEnvironmentVariable("PATH"),
                File.Exists);
            var rubyExecutable = RubyExecutableWithin(chefInstallDirectory);
            var chefClientLoaderFile = ChefClientLoaderWithin(chefInstallDirectory);
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo(rubyExecutable)
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    Arguments = chefClientLoaderFile
                },
            };
            process.OutputDataReceived += ProcessOnOutputDataReceived;
            process.ErrorDataReceived += ProcessOnErrorDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            Logger.LogInformation("Chef started; waiting for exit");
            process.WaitForExit();
            Logger.LogInformation($"Chef exited at {process.ExitTime} with status of {process.ExitCode}");
        }

        public static string ChefClientLoaderWithin(string chefInstallDirectory)
        {
            return $@"{chefInstallDirectory}\bin\chef-client";
        }


        private static void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Logger.LogCritical($"STANDARD ERROR: {e.Data}");
        }

        private static void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                ChefLogEntry.Parse(e.Data).Log();

            }
            catch (Exception exception)
            {
                Logger.LogCritical(default(EventId), exception, $"Could not parse and log {e.Data}");
            }
        }

        public static string FindChefInstallationDirectory(string environmentPath, Func<string, bool> fileExists)
        {
            var paths = environmentPath.Split(';');
            const string chefClientBat = "chef-client.bat";
            var batchFilePath = paths
                .Select(x => Path.Combine(x, chefClientBat))
                .FirstOrDefault(File.Exists);
            if (batchFilePath == null)
            {
                Logger.LogWarning($"Could not find {chefClientBat} in the path {environmentPath}");
                return null;
            }
            var binDirectory = Directory.GetParent(batchFilePath);
            var installDirectory = Directory.GetParent(binDirectory.FullName);
            return installDirectory.FullName;
        }

        public static string RubyExecutableWithin(string chefInstallPath)
        {
            return Path.Combine(chefInstallPath, @"embedded\bin\ruby.exe");
        }
    }
}