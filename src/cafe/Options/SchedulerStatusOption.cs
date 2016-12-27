using cafe.Client;
using cafe.CommandLine;
using NLog;

namespace cafe.Options
{
    public class SchedulerStatusOption : Option
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ClientFactory _clientFactory;

        public SchedulerStatusOption(ClientFactory clientFactory) : base(new OptionSpecification("scheduler", "status"),
            "Gets the status of the scheduler")
        {
            _clientFactory = clientFactory;
        }

        protected override void RunCore(string[] args)
        {
            var status = _clientFactory.RestClientForSchedulerServer().GetStatus().Result;
            Presenter.ShowMessage($"Is Running: {status.IsRunning}", Logger);
            Presenter.ShowMessage($"Queued tasks: {status.QueuedTasks}", Logger);
        }
    }
}