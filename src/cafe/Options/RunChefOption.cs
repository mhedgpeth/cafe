using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Options
{
    public class RunChefOption : ChefOption
    {
        public RunChefOption(ClientFactory clientFactory, SchedulerWaiter schedulerWaiter)
            : base(clientFactory, schedulerWaiter, new OptionSpecification("chef", "run"), "runs chef")
        {
        }

        protected override Task<ScheduledTaskStatus> RunCore(IChefServer chefServer, string[] args)
        {
            return chefServer.RunChef();
        }
    }
}