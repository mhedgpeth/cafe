using System;
using System.Threading.Tasks;
using cafe.Client;
using cafe.Shared;

namespace cafe.Options
{
    public class DownloadChefOption : ChefJobOption
    {

        public DownloadChefOption(Func<IChefServer> chefServerFactory, ISchedulerWaiter schedulerWaiter)
            : base(chefServerFactory, schedulerWaiter,
                "downloads the provided version of chef")
        {
        }

        protected override Task<JobRunStatus> RunJobCore(IChefServer chefServer, string[] args)
        {
            return chefServer.DownloadChef(FindVersion(args));
        }

        public static string FindVersion(string[] args)
        {
            return args[2];
        }

        protected override string ToDescription(string[] args)
        {
            return $"Downloading Chef {FindVersion(args)}";
        }
    }
}