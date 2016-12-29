using cafe.CommandLine;
using cafe.LocalSystem;
using cafe.Shared;
using NLog;

namespace cafe.Options.Server
{
    public class CafeWindowsServiceStatusOption : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(CafeWindowsServiceStatusOption).FullName);

        private readonly ServiceStatusProvider _serviceStatusProvider;


        public CafeWindowsServiceStatusOption(ProcessExecutor processExecutor, IFileSystem fileSystem)
            : base(new OptionSpecification("service", "status"), "gets the status of the cafe windows service")
        {
            _serviceStatusProvider = new ServiceStatusProvider(processExecutor, fileSystem);
        }

        protected override string ToDescription(string[] args)
        {
            return "Determining Cafe Windows Service Status";
        }

        protected override Result RunCore(string[] args)
        {
            var serviceName = CafeServerWindowsServiceOptions.ServiceName;
            var status = _serviceStatusProvider.DetermineStatusDescription(serviceName);
            Presenter.ShowMessage($"{serviceName} status is {status}", Logger);
            return Result.Successful();
        }
    }
}