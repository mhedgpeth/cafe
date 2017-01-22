using System.Diagnostics;
using cafe.CommandLine;
using cafe.Shared;
using DasMulli.Win32.ServiceUtils;
using NLog;

namespace cafe.Options.Server
{
    public class RegisterServerWindowsServiceOption : Option
    {
        private static readonly Logger Logger =
            LogManager.GetLogger(typeof(RegisterServerWindowsServiceOption).FullName);

        public RegisterServerWindowsServiceOption() : base("Registers cafe as a service")
        {
        }

        protected override string ToDescription(string[] args)
        {
            return "Registering Cafe to run as a Windows Service";
        }

        protected override Result RunCore(string[] args)
        {
            var fullPathToThis = Process.GetCurrentProcess().MainModule.FileName;
            var fullServiceCommand = $"{fullPathToThis} server --run-as-service";

            // Do not use LocalSystem in production.. but this is good for demos as LocalSystem will have access to some random git-clone path
            new Win32ServiceManager()
                .CreateService(CafeServerWindowsServiceOptions.ServiceName,
                    CafeServerWindowsServiceOptions.ServiceDisplayName,
                    CafeServerWindowsServiceOptions.ServiceDescription,
                    fullServiceCommand,
                    Win32ServiceCredentials.LocalSystem,
                    autoStart: true,
                    startImmediately: true,
                    errorSeverity: ErrorSeverity.Normal);

            Presenter.ShowMessage(
                $@"Successfully registered and started service '{
                        CafeServerWindowsServiceOptions.ServiceDisplayName
                    }' ({CafeServerWindowsServiceOptions.ServiceDisplayName})",
                Logger);
            return Result.Successful();
        }
    }
}