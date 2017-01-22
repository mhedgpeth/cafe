using System.Threading.Tasks;
using cafe.Client;
using cafe.Shared;

namespace cafe.Options
{
    public class RunChefOption : ChefOption
    {
        public RunChefOption(ClientFactory clientFactory, SchedulerWaiter schedulerWaiter)
            : base(clientFactory, schedulerWaiter, "runs chef")
        {
        }

        protected override Task<JobRunStatus> RunCore(IChefServer chefServer, string[] args)
        {
            return chefServer.RunChef();
        }

        protected override string ToDescription(string[] args)
        {
            return "Running Chef";
        }
    }
}