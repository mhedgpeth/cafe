using System.Diagnostics;
using DasMulli.Win32.ServiceUtils;
using NLog;

namespace cafe.CommandLine.Options
{
    public class RegisterServerWindowsServiceOption : Option
    {
        private readonly string _serviceName;
        private readonly string _serviceDisplayName;
        private readonly string _serviceDescription;

        private static readonly Logger Logger =
            LogManager.GetLogger(typeof(RegisterServerWindowsServiceOption).FullName);

        public RegisterServerWindowsServiceOption(string serviceName, string serviceDisplayName, string serviceDescription) : base("Registers cafe as a service")
        {
            _serviceName = serviceName;
            _serviceDisplayName = serviceDisplayName;
            _serviceDescription = serviceDescription;
        }

        protected override string ToDescription(Argument[] args)
        {
            return $"Registering {_serviceName} to run as a Windows Service";
        }

        protected override Result RunCore(Argument[] args)
        {
            var fullPathToThis = Process.GetCurrentProcess().MainModule.FileName;
            var fullServiceCommand = $"{fullPathToThis} server --run-as-service";

            // Do not use LocalSystem in production.. but this is good for demos as LocalSystem will have access to some random git-clone path
            new Win32ServiceManager()
                .CreateService(_serviceName,
                    _serviceDisplayName,
                    _serviceDescription,
                    fullServiceCommand,
                    Win32ServiceCredentials.LocalSystem,
                    autoStart: true,
                    startImmediately: true,
                    errorSeverity: ErrorSeverity.Normal);

            Presenter.ShowMessage(
                $@"Successfully registered and started service '{
                        _serviceDisplayName
                    }' ({_serviceDisplayName})",
                Logger);
            return Result.Successful();
        }
    }
}