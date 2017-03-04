using System;
using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Options.Chef
{
    public class RunChefOption : RunJobOption<IChefServer>
    {
        public RunChefOption(Func<IChefServer> chefServerFactory, ISchedulerWaiter schedulerWaiter)
            : base(chefServerFactory, schedulerWaiter, "runs chef")
        {
        }

        protected override Task<JobRunStatus> RunJobCore(IChefServer chefServer, Argument[] args)
        {
            return chefServer.RunChef();
        }

        protected override string ToDescription(Argument[] args)
        {
            return "Running Chef";
        }
    }
}