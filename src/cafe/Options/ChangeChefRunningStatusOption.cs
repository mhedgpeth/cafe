using System;
using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
using NLog;
using NLog.Fluent;

namespace cafe.Options
{
    public class ChangeChefRunningStatusOption : SchedulerOption
    {
        private readonly string _command;
        private readonly string _commandDescription;
        private readonly Func<IChefServer, Task<ServerStatus>> _serverAction;
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChangeChefRunningStatusOption).FullName);

        private ChangeChefRunningStatusOption(Func<IChefServer> schedulerServerProvider, string command,
            string commandDescription,
            Func<IChefServer, Task<ServerStatus>> serverAction)
            : base(schedulerServerProvider, $"{command} chef")
        {
            _command = command;
            _commandDescription = commandDescription;
            _serverAction = serverAction;
        }

        protected override string ToDescription(string[] args)
        {
            return $"{_commandDescription} Chef";
        }

        protected override Result RunCore(IChefServer server, string[] args)
        {
            var status = _serverAction(server).Result;
            if (status == null)
            {
                Presenter.ShowMessage(
                    $"There is no recurring task for chef on the server, so there is nothing to {_command}.", Logger);
            }
            else
            {
                Log.Info($"Result of {_command}: {status}");
                Presenter.ShowMessage(
                    status.ToString(), Logger);
            }
            return Result.Successful();
        }

        public static ChangeChefRunningStatusOption CreatePauseChefOption(
            Func<IChefServer> chefServerProvider)
        {
            return new ChangeChefRunningStatusOption(chefServerProvider, "pause", "Pausing",
                server => server.Pause());
        }

        public static ChangeChefRunningStatusOption CreateResumeChefOption(
            Func<IChefServer> chefServerProvider)
        {
            return new ChangeChefRunningStatusOption(chefServerProvider, "resume", "Resuming",
                server => server.Resume());
        }
    }
}