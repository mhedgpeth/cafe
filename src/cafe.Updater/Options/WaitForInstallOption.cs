using System;
using System.IO;
using System.Threading;
using cafe.CommandLine;
using cafe.CommandLine.LocalSystem;
using NLog;

namespace cafe.Updater.Options
{
    public class WaitForInstallOption : Option
    {
        private readonly IFileSystemCommands _commands;
        private static readonly Logger Logger = LogManager.GetLogger(typeof(WaitForInstallOption).FullName);

        public WaitForInstallOption(IFileSystemCommands commands) : base("Waiting for install to complete")
        {
            _commands = commands;
        }

        protected override string ToDescription(Argument[] args)
        {
            return "Waiting for install to complete";
        }

        protected override Result RunCore(Argument[] args)
        {
            var installer = args.FindValueFromLabel("installer:").Value;
            DateTime start = DateTime.Now;
            TimeSpan timeout = TimeSpan.FromMinutes(5);

            bool installComplete = false;
            while (DateTime.Now < start + timeout)
            {
                Presenter.ShowMessage($"Checking to see if installer {installer} has been procesed yet", Logger);
                if (!_commands.FileExists(Path.Combine("staging", installer)))
                {
                    Presenter.ShowMessage("Installer has been processed, so install is complete!", Logger);
                    installComplete = true;
                    break;
                }
                else
                {
                    Presenter.ShowMessage("Installer has not yet been processed, Waiting for 5 seconds.", Logger);
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }
            return installComplete
                ? Result.Successful()
                : Result.Failure($"After waiting {timeout.TotalSeconds} seconds, installer didn't complete");
        }
    }
}
