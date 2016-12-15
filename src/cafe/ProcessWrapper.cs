using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace cafe
{
    public class ProcessWrapper : IProcess
    {
        private readonly Process _process = new Process();

        public void Start()
        {
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
            _process.BeginOutputReadLine();
        }

        public void BeginErrorReadLine()
        {
            _process.BeginErrorReadLine();
        }

        public void WaitForExit()
        {
            _process.WaitForExit();
        }
    }
}