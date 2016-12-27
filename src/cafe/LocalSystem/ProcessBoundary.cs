using System;
using System.Collections.Generic;
using System.Diagnostics;
using NLog;

namespace cafe.LocalSystem
{
    public class ProcessBoundary : IProcess
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Process _process = new Process();

        public void Start()
        {
            Logger.Debug($"Starting {this}");
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
            Logger.Debug($"{this} is reading from standard output with redirection set to {StartInfo.RedirectStandardOutput}");
            _process.BeginOutputReadLine();
        }

        public void BeginErrorReadLine()
        {
            Logger.Debug($"{this} is reading from standard error with redirection set to {StartInfo.RedirectStandardError}");
            _process.BeginErrorReadLine();
        }

        public void WaitForExit()
        {
            Logger.Debug($"Waiting for process {this} to exit");
            _process.WaitForExit();
            Logger.Debug($"{this} has exited with exit code {_process.ExitCode}");
        }

        public override string ToString()
        {
            return $"Process from file {_process.StartInfo.FileName} with arguments {StartInfo.Arguments}";
        }
    }
}