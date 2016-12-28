using System;
using System.ComponentModel;
using System.Diagnostics;
using cafe.Shared;
using NLog;
using NLog.Fluent;

namespace cafe.LocalSystem
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
            var process = _processCreator();
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

            Logger.Info("Chef started; waiting for exit");
            process.WaitForExit();
            Logger.Info($"Chef exited at {process.ExitTime} with status of {process.ExitCode}");
            return process.ExitCode == 0 ? Result.Successful() : Result.Failure($"Process {process} exited with code {process.ExitCode}");
        }
    }
}