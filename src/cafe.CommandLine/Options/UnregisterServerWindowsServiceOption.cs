using DasMulli.Win32.ServiceUtils;
using NLog;

namespace cafe.CommandLine.Options
{
    public class UnregisterServerWindowsServiceOption : Option
    {
        private readonly string _serviceName;
        private readonly string _serviceDisplayName;

        private static readonly Logger Logger =
            LogManager.GetLogger(typeof(UnregisterServerWindowsServiceOption).FullName);

        public UnregisterServerWindowsServiceOption(string serviceName, string serviceDisplayName) : base("unregisters cafe to run as a service")
        {
            _serviceName = serviceName;
            _serviceDisplayName = serviceDisplayName;
        }

        protected override string ToDescription(Argument[] args)
        {
            return $"Unregistering {_serviceDisplayName} to run as a Windows Service";
        }

        protected override Result RunCore(Argument[] args)
        {
            new Win32ServiceManager().DeleteService(_serviceName);
            Presenter.ShowMessage(
                $@"Successfully unregistered service '{_serviceName}' ({_serviceDisplayName})",
                Logger);
            return Result.Successful();
        }
    }
}