using System;
using System.Diagnostics;
using cafe.Shared;
using NLog;

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
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            Logger.Info("Chef started; waiting for exit");
            process.WaitForExit();
            Logger.Info($"Chef exited at {process.ExitTime} with status of {process.ExitCode}");
            return process.ExitCode == 0 ? Result.Successful() : Result.Failure($"Process {process} exited with code {process.ExitCode}");
        }
    }
}