using System;
using System.Threading.Tasks;
using cafe.Client;
using cafe.Shared;

namespace cafe.Options
{
    public class InstallChefOption : ChefJobOption
    {
        public InstallChefOption(Func<IChefServer> chefServerCreator, ISchedulerWaiter schedulerWaiter)
            : base(chefServerCreator, schedulerWaiter,
                "installs or upgrades chef to the specified version")
        {
        }

        protected override Task<JobRunStatus> RunJobCore(IChefServer chefServer, string[] args)
        {
            return chefServer.InstallChef(DownloadChefOption.FindVersion(args));
        }

        protected override string ToDescription(string[] args)
        {
            return $"Installing Chef {DownloadChefOption.FindVersion(args)}";
        }
    }
}