using System;
using System.Threading;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options.Server
{
    public class ServerInteractiveOption : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ServerInteractiveOption).FullName);

        public ServerInteractiveOption() : base("Starts cafe in server mode to be run on the console")
        {
        }

        protected override Result RunCore(string[] args)
        {
            var cafeServerWindowsService = new CafeServerWindowsService();
            cafeServerWindowsService.Start(new string[0], () => { });
            Presenter.ShowMessage("Running interactively, press ctrl+c to stop", Logger);
            // don't use Console.ReadLine because it interferes with
            // console redirection done to keep an eye on processes
            // that this kicks off
            new AutoResetEvent(false).WaitOne();
            cafeServerWindowsService.Stop();
            return Result.Successful();
        }

        protected override string ToDescription(string[] args)
        {
            return "Starting Cafe in Server Mode";
        }
    }
}