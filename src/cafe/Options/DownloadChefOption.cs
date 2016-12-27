using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Options
{
    public class DownloadChefOption : ChefOption
    {
        private readonly ClientFactory _clientFactory;
        private readonly SchedulerWaiter _schedulerWaiter;

        public DownloadChefOption(ClientFactory clientFactory, SchedulerWaiter schedulerWaiter)
            : base(clientFactory, schedulerWaiter,
                new OptionSpecification(OptionValueSpecification.ForExactValue("chef"),
                    OptionValueSpecification.ForExactValue("download"),
                    OptionValueSpecification.ForVersion()),
                "downloads the provided version of chef")
        {
            _clientFactory = clientFactory;
            _schedulerWaiter = schedulerWaiter;
        }

        protected override Task<ScheduledTaskStatus> RunCore(IChefServer chefServer, string[] args)
        {
            return chefServer.DownloadChef(args[2]);
        }
    }
}