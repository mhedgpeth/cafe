using System;
using System.Diagnostics;

namespace cafe.CommandLine.LocalSystem
{
    public interface IProcess : IDisposable {
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