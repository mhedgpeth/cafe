using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options
{
    public abstract class ChefOption : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefOption).FullName);

        private readonly ClientFactory _clientFactory;
        private readonly SchedulerWaiter _schedulerWaiter;

        protected ChefOption(ClientFactory clientFactory, SchedulerWaiter schedulerWaiter,
            OptionSpecification optionSpecification, string helpText)
            : base(optionSpecification, helpText)
        {
            _clientFactory = clientFactory;
            _schedulerWaiter = schedulerWaiter;
        }

        protected override void RunCore(string[] args)
        {
            var client = _clientFactory.RestClientForChefServer();
            var status = RunCore(client, args).Result;
            var finalStatus = _schedulerWaiter.WaitForTaskToComplete(status);
            Presenter.ShowMessage($"Finished running {finalStatus.Description}", Logger);
        }


        protected abstract Task<ScheduledTaskStatus> RunCore(IChefServer chefServer, string[] args);
    }
}