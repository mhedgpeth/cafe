using System;
using System.ComponentModel;
using System.Diagnostics;
using NLog;

namespace cafe.CommandLine.LocalSystem
{
    public class ProcessExecutor
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ProcessExecutor).FullName);

        private readonly Func<IProcess> _processCreator;

        public ProcessExecutor(Func<IProcess> processCreator)
        {
            _processCreator = processCreator;
        }

        public Result ExecuteAndWaitForExit(string filename, string processArguments,
            EventHandler<string> processOnOutputDataReceived,
            EventHandler<string> processOnErrorDataReceived)
        {
            using (var process = _processCreator())
            {
                process.StartInfo = new ProcessStartInfo(filename)
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    Arguments = processArguments
                };
                process.OutputDataReceived += processOnOutputDataReceived;
                process.ErrorDataReceived += processOnErrorDataReceived;
                try
                {
                    Logger.Debug($"Starting process {filename} with arguments {processArguments}");
                    process.Start();

                }
                catch (Win32Exception ex)
                {
                    Logger.Error(ex, $"An unexpected error occurred while trying to start {filename}");
                    return Result.Failure(
                        $"Process {filename} could not run because it requires elevated privileges. Make sure the user running this server has the appropriate rights");
                }
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                Logger.Info($"Process {filename} with arguments {processArguments} started; waiting for exit");
                process.WaitForExit();
                Logger.Info($"Process {filename} exited at {process.ExitTime} with status of {process.ExitCode}");
                return process.ExitCode == 0
                    ? Result.Successful()
                    : Result.Failure($"Process {process} exited with code {process.ExitCode}");
            }
        }
    }
}