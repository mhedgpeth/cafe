using System;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
using NLog;
using NLog.Fluent;

namespace cafe.Options
{
    public class SchedulerStatusOption : Option
    {
        private readonly Func<ISchedulerServer> _schedulerServerProvider;
        private static readonly Logger Logger = LogManager.GetLogger(typeof(SchedulerStatusOption).FullName);

        public SchedulerStatusOption(Func<ISchedulerServer> schedulerServerProvider) : base(
            new OptionSpecification("status"),
            "Gets the status of the cafe server")
        {
            _schedulerServerProvider = schedulerServerProvider;
        }

        protected override Result RunCore(string[] args)
        {
            try
            {
                var status = _schedulerServerProvider().GetStatus().Result;
                Presenter.ShowMessage($"Is Running: {status.IsRunning}", Logger);
                Presenter.ShowMessage($"Queued tasks: {status.QueuedTasks}", Logger);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "An exception occurred while trying to get the status of the server");
                Presenter.ShowMessage("The server is not currently running", Logger);
            }
            return Result.Successful();
        }

        protected override string ToDescription(string[] args)
        {
            return "Determining Status";
        }
    }
}