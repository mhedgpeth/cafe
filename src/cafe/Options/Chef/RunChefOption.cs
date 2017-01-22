using System;
using System.Threading.Tasks;
using cafe.Client;
using cafe.Shared;

namespace cafe.Options
{
    public class RunChefOption : ChefJobOption
    {
        public RunChefOption(Func<IChefServer> chefServerFactory, SchedulerWaiter schedulerWaiter)
            : base(chefServerFactory, schedulerWaiter, "runs chef")
        {
        }

        protected override Task<JobRunStatus> RunJobCore(IChefServer chefServer, string[] args)
        {
            return chefServer.RunChef();
        }

        protected override string ToDescription(string[] args)
        {
            return "Running Chef";
        }
    }
}