using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace cafe.Test
{
    public class FakeProcess : IProcess
    {
        public void Start()
        {
        }

        public event EventHandler<string> ErrorDataReceived;
        public ProcessStartInfo StartInfo { get; set; }
        public DateTime ExitTime { get; }
        public int ExitCode { get; }
        public List<string> OutputDataReceivedDuringWaitForExit { get; } = new List<string>();
        public List<string> ErrorDataReceivedDuringWaitForExit { get; } = new List<string>();

        public event EventHandler<string> OutputDataReceived;

        public void BeginOutputReadLine()
        {
        }

        public void BeginErrorReadLine()
        {
        }

        public void WaitForExit()
        {
            OutputDataReceivedDuringWaitForExit.ForEach(OnOutputDataReceived);
            ErrorDataReceivedDuringWaitForExit.ForEach(OnErrorDataReceived);
        }

        protected virtual void OnOutputDataReceived(string e)
        {
            OutputDataReceived?.Invoke(this, e);
        }

        protected virtual void OnErrorDataReceived(string e)
        {
            ErrorDataReceived?.Invoke(this, e);
        }
    }
}