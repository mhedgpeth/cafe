using System;
using System.Threading;
using DasMulli.Win32.ServiceUtils;
using NLog;

namespace cafe.CommandLine.Options
{
    public class ServerInteractiveOption : Option
    {
        private readonly string _application;
        private readonly Func<IWin32Service> _windowsServiceCreator;
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ServerInteractiveOption).FullName);

        public ServerInteractiveOption(string application, Func<IWin32Service> windowsServiceCreator) : base($"Starts {application} in server mode to be run on the console")
        {
            _application = application;
            _windowsServiceCreator = windowsServiceCreator;
        }

        protected override Result RunCore(Argument[] args)
        {
            var windowsService = _windowsServiceCreator();
            windowsService.Start(new string[0], () => { });
            Presenter.ShowMessage("Running interactively, press ctrl+c to stop", Logger);
            // don't use Console.ReadLine because it interferes with
            // console redirection done to keep an eye on processes
            // that this kicks off
            new AutoResetEvent(false).WaitOne();
            windowsService.Stop();
            return Result.Successful();
        }

        protected override string ToDescription(Argument[] args)
        {
            return $"Starting {_application} in Server Mode";
        }
    }
}