using cafe.CommandLine.LocalSystem;
using NLog;

namespace cafe.CommandLine.Options
{
    public class CafeWindowsServiceStatusOption : Option
    {
        private readonly string _serviceName;
        private static readonly Logger Logger = LogManager.GetLogger(typeof(CafeWindowsServiceStatusOption).FullName);

        private readonly ServiceStatusProvider _serviceStatusProvider;


        public CafeWindowsServiceStatusOption(ProcessExecutor processExecutor, IFileSystem fileSystem, string serviceName)
            : base("gets the status of the cafe windows service")
        {
            _serviceName = serviceName;
            _serviceStatusProvider = new ServiceStatusProvider(processExecutor, fileSystem);
        }

        protected override string ToDescription(Argument[] args)
        {
            return "Determining Cafe Windows Service Status";
        }

        protected override Result RunCore(Argument[] args)
        {
            var status = _serviceStatusProvider.DetermineStatusDescription(_serviceName);
            Presenter.ShowMessage($"{_serviceName} status is {status}", Logger);
            return Result.Successful();
        }
    }
}