using System;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options
{
    public class ServerInteractiveOption : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ServerInteractiveOption).FullName);

        public ServerInteractiveOption() : base(new OptionSpecification("server"), "Starts cafe in server mode to be run on the console")
        {
        }

        protected override Result RunCore(string[] args)
        {
            var cafeServerWindowsService = new CafeServerWindowsService();
            cafeServerWindowsService.Start(new string[0], () => { });
            Presenter.ShowMessage("Running interactively, press enter to stop", Logger);
            Console.ReadLine();
            cafeServerWindowsService.Stop();
            return Result.Successful();
        }

        protected override string ToDescription(string[] args)
        {
            return "Starting Cafe in Server Mode";
        }
    }
}