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

        private readonly IClientFactory _clientFactory;
        private readonly ISchedulerWaiter _schedulerWaiter;

        protected ChefOption(IClientFactory clientFactory, ISchedulerWaiter schedulerWaiter,
            OptionSpecification optionSpecification, string helpText)
            : base(optionSpecification, helpText)
        {
            _clientFactory = clientFactory;
            _schedulerWaiter = schedulerWaiter;
        }

        protected override Result RunCore(string[] args)
        {
            var client = _clientFactory.RestClientForChefServer();
            var status = RunCore(client, args).Result;
            var finalStatus = _schedulerWaiter.WaitForTaskToComplete(status);
            Logger.Info($"Finished running {finalStatus.Description}");
            return finalStatus.Result;
        }

        protected abstract Task<ScheduledTaskStatus> RunCore(IChefServer chefServer, string[] args);
    }
}