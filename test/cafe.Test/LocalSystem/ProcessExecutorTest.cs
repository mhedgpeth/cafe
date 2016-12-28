using System.ComponentModel;
using cafe.LocalSystem;
using FluentAssertions;
using Moq;
using Xunit;

namespace cafe.Test.LocalSystem
{
    public class ProcessExecutorTest
    {
        [Fact]
        public void ExecuteAndWaitForExit_ShouldReturnFailureIfThrowsWin32Exception()
        {
            var process = new Mock<IProcess>();
            process.Setup(p => p.Start()).Throws<Win32Exception>();
            var processExecutor = new ProcessExecutor(() => process.Object);

            const string processName = "process.exe";
            var result = processExecutor.ExecuteAndWaitForExit(processName, "arg1 arg2", DoNothing, DoNothing);

            result.IsSuccess.Should().BeFalse("because the process executor threw an exception");
            result.FailureDescription.Should()
                .Be(
                    $"Process {processName} could not run because it requires elevated privileges. Make sure the user running this server has the appropriate rights");
        }

        private void DoNothing(object sender, string e)
        {
        }
    }
}