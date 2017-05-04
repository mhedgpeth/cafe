using System;
using DasMulli.Win32.ServiceUtils;
using NLog;

namespace cafe.CommandLine.Options
{
    public class ServerWindowsServiceOption : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ServerWindowsServiceOption).FullName);

        private readonly string _application;
        private readonly Func<IWin32Service> _windowsServiceCreator;

        public ServerWindowsServiceOption(string application, Func<IWin32Service> windowsServiceCreator) : base($"Runs {application} in service mode")
        {
            _application = application;
            _windowsServiceCreator = windowsServiceCreator;
        }

        protected override string ToDescription(Argument[] args)
        {
            return "Starting Cafe in Service Mode as a Windows Service";
        }

        protected override Result RunCore(Argument[] args)
        {
            Logger.Info($"Creating service host for {_application}");
            var windowsService = _windowsServiceCreator();
            var serviceHost = new Win32ServiceHost(windowsService);
            Logger.Debug("Service host created, running");
            serviceHost.Run();
            Logger.Debug($"Finished running service host for {_application}");
            return Result.Successful();
        }
    }
}