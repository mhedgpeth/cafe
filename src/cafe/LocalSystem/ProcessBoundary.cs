using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace cafe.LocalSystem
{
    public class ProcessBoundary : IProcess
    {
        private static ILogger Logger { get; } =
          ApplicationLogging.CreateLogger<ProcessBoundary>();

        private readonly Process _process = new Process();

        public void Start()
        {
            Logger.LogDebug($"Starting process {_process.ProcessName} on {_process.MachineName} with arguments {_process.StartInfo.Arguments}");
            _process.Start();
        }

        private readonly IDictionary<EventHandler<string>, DataReceivedEventHandler> _delegates =
            new Dictionary<EventHandler<string>, DataReceivedEventHandler>();

        public event EventHandler<string> ErrorDataReceived
        {
            add
            {
                _delegates.Add(value, (sender, args) => value(sender, args.Data));
                _process.ErrorDataReceived += _delegates[value];
            }
            remove { _process.ErrorDataReceived -= _delegates[value]; }
        }

        public ProcessStartInfo StartInfo
        {
            get { return _process.StartInfo; }
            set { _process.StartInfo = value; }
        }

        public DateTime ExitTime => _process.ExitTime;

        public int ExitCode => _process.ExitCode;

        public event EventHandler<string> OutputDataReceived
        {
            add
            {
                _delegates.Add(value, (sender, args) => value(sender, args.Data));
                _process.OutputDataReceived += _delegates[value];
            }
            remove { _process.OutputDataReceived -= _delegates[value]; }
        }

        public void BeginOutputReadLine()
        {
            Logger.LogDebug($"Process {_process.ProcessName} is reading from standard output with redirection set to {StartInfo.RedirectStandardOutput}");
            _process.BeginOutputReadLine();
        }

        public void BeginErrorReadLine()
        {
            Logger.LogDebug($"Process {_process.ProcessName} is reading from standard error with redirection set to {StartInfo.RedirectStandardError}");
            _process.BeginErrorReadLine();
        }

        public void WaitForExit()
        {
            Logger.LogDebug($"Waiting for process {_process.ProcessName} to exit");
            _process.WaitForExit();
            Logger.LogDebug($"Process {_process.ProcessName} has exited with exit code {_process.ExitCode}");
        }

        public override string ToString()
        {
            return $"Process {_process.ProcessName} from file {_process.StartInfo.FileName} with arguments {StartInfo.Arguments}";
        }
    }
}