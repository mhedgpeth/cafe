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
        private readonly Func<ISchedulerServer, Task<RecurringTaskStatus>> _serverAction;
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChangeChefRunningStatusOption).FullName);

        public ChangeChefRunningStatusOption(Func<ISchedulerServer> schedulerServerProvider, string command,
            string commandDescription,
            Func<ISchedulerServer, Task<RecurringTaskStatus>> serverAction)
            : base(schedulerServerProvider, new OptionSpecification("chef", command), $"{command} chef")
        {
            _command = command;
            _commandDescription = commandDescription;
            _serverAction = serverAction;
        }

        protected override string ToDescription(string[] args)
        {
            return $"{_commandDescription} Chef";
        }

        protected override Result RunCore(ISchedulerServer server, string[] args)
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
            Func<ISchedulerServer> schedulerServerProvider)
        {
            return new ChangeChefRunningStatusOption(schedulerServerProvider, "pause", "Pausing",
                server => server.PauseRecurringTask("chef"));
        }

        public static ChangeChefRunningStatusOption CreateResumeChefOption(
            Func<ISchedulerServer> schedulerServerProvider)
        {
            return new ChangeChefRunningStatusOption(schedulerServerProvider, "resume", "Resuming",
                server => server.ResumeRecurringTask("chef"));
        }
    }
}