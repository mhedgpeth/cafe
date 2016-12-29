using cafe.CommandLine;
using cafe.Shared;
using DasMulli.Win32.ServiceUtils;
using NLog;

namespace cafe.Options.Server
{
    public class UnregisterServerWindowsServiceOption : Option
    {
        private static readonly Logger Logger =
            LogManager.GetLogger(typeof(UnregisterServerWindowsServiceOption).FullName);

        public UnregisterServerWindowsServiceOption() : base(new OptionSpecification("service", "unregister"), "unregisters cafe to run as a service")
        {
        }

        protected override string ToDescription(string[] args)
        {
            return "Unregistering Cafe to run as a Windows Service";
        }

        protected override Result RunCore(string[] args)
        {
            new Win32ServiceManager().DeleteService(CafeServerWindowsServiceOptions.ServiceName);
            Presenter.ShowMessage(
                $@"Successfully unregistered service '{CafeServerWindowsServiceOptions.ServiceDisplayName}' ({CafeServerWindowsServiceOptions.ServiceDisplayName})",
                Logger);
            return Result.Successful();
        }
    }
}