using System;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options
{
    public class ShowChefStatusOption : SchedulerOption
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChangeChefRunningStatusOption).FullName);

        public ShowChefStatusOption(Func<ISchedulerServer> schedulerServerProvider)
            : base(schedulerServerProvider, new OptionSpecification("chef", "status"), "shows chef status")
        {
        }

        protected override string ToDescription(string[] args)
        {
            return "Showing Chef Status";
        }

        protected override Result RunCore(ISchedulerServer server, string[] args)
        {
            var status = server.GetRecurringTaskStatus("chef").Result;

            var message = status != null ? status.ToString() : "Chef is not scheduled to regularly run";
            Presenter.ShowMessage(message, Logger);
            return Result.Successful();
        }
    }
}