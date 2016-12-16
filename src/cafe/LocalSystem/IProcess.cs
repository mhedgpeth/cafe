using System;
using System.Diagnostics;

namespace cafe.LocalSystem
{
    public interface IProcess {
        void Start();
        event EventHandler<string> ErrorDataReceived;
        ProcessStartInfo StartInfo { get; set; }
        DateTime ExitTime { get; }
        int ExitCode { get; }
        event EventHandler<string> OutputDataReceived;
        void BeginOutputReadLine();
        void BeginErrorReadLine();
        void WaitForExit();
    }
}