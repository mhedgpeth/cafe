using System.IO;
using cafe.CommandLine;
using cafe.LocalSystem;
using cafe.Shared;
using NLog;

namespace cafe.Options.Server
{
    public class ChangeStateForCafeWindowsServiceOption : Option
    {
        private static readonly Logger Logger =
            LogManager.GetLogger(typeof(ChangeStateForCafeWindowsServiceOption).FullName);

        private readonly string _command;
        private readonly ServiceStatus _waitFor;
        private readonly ProcessExecutor _processExecutor;
        private readonly IFileSystem _fileSystem;
        private readonly ServiceStatusWaiter _serviceStatusWaiter;

        private ChangeStateForCafeWindowsServiceOption(string command, ServiceStatus waitFor,
            ProcessExecutor processExecutor, IFileSystem fileSystem,
            ServiceStatusWaiter serviceStatusWaiter) : base($"{command} service")
        {
            _command = command;
            _waitFor = waitFor;
            _processExecutor = processExecutor;
            _fileSystem = fileSystem;
            _serviceStatusWaiter = serviceStatusWaiter;
        }

        protected override string ToDescription(string[] args)
        {
            return "Starting Cafe Windows Service";
        }

        protected override Result RunCore(string[] args)
        {
            var descriptions = ServiceStatusProvider.DescribeWindowsStatuses();
            var waitForDescription = descriptions[_waitFor];
            string serviceName = CafeServerWindowsServiceOptions.ServiceName;
            var fullPath =
                _fileSystem.FindInstallationDirectoryInPathContaining(ServiceStatusProvider
                    .ServiceControllerExecutable, @"C:\windows\System32");
            Presenter.ShowMessage($"Executing command to {_command} the service {serviceName}", Logger);
            _processExecutor.ExecuteAndWaitForExit(
                Path.Combine(fullPath, ServiceStatusProvider.ServiceControllerExecutable),
                $"{_command} {serviceName}",
                LogInformation, LogError);
            Presenter.ShowMessage($"Waiting for {serviceName} to be {waitForDescription}", Logger);
            var status = _serviceStatusWaiter.WaitFor(_waitFor);
            var statusDescription = descriptions[status];
            Presenter.ShowMessage($"Service {serviceName} status is now {statusDescription}", Logger);
            return status == _waitFor
                ? Result.Successful()
                : Result.Failure(
                    $"Expecting {_command} command to put the service in '{waitForDescription}' status but instead it is in '{statusDescription}' status.");
        }

        private static void LogError(object sender, string e)
        {
            Logger.Error(e);
        }

        private static void LogInformation(object sender, string e)
        {
            Logger.Info(e);
        }

        public static ChangeStateForCafeWindowsServiceOption StartCafeWindowsServiceOption(
            ProcessExecutor processExecutor, IFileSystem fileSystem, ServiceStatusWaiter serviceStatusWaiter)
        {
            return new ChangeStateForCafeWindowsServiceOption("start", ServiceStatus.Running, processExecutor,
                fileSystem, serviceStatusWaiter);
        }

        public static ChangeStateForCafeWindowsServiceOption StopCafeWindowsServiceOption(
            ProcessExecutor processExecutor, IFileSystem fileSystem, ServiceStatusWaiter serviceStatusWaiter)
        {
            return new ChangeStateForCafeWindowsServiceOption("stop", ServiceStatus.Stopped, processExecutor,
                fileSystem, serviceStatusWaiter);
        }
    }
}