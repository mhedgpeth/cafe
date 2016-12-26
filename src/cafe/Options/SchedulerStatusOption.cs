using cafe.Client;
using cafe.CommandLine;
using Microsoft.Extensions.Logging;

namespace cafe.Options
{
    public class SchedulerStatusOption : Option
    {
        private static ILogger Logger { get; } =
            ApplicationLogging.CreateLogger<SchedulerStatusOption>();

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