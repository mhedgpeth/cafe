using System;
using System.Diagnostics;
using NLog;

namespace cafe.LocalSystem
{
    public class ProcessExecutor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Func<IProcess> _processCreator;

        public ProcessExecutor(Func<IProcess> processCreator)
        {
            _processCreator = processCreator;
        }

        public void ExecuteAndWaitForExit(string filename, string processArguments,
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
        }
    }
}