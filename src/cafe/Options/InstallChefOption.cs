using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Options
{
    public class InstallChefOption : ChefOption
    {
        public InstallChefOption(ClientFactory clientFactory, SchedulerWaiter schedulerWaiter)
            : base(clientFactory, schedulerWaiter,
                new OptionSpecification(OptionValueSpecification.ForExactValue("chef"),
                    OptionValueSpecification.ForExactValues("install", "upgrade"), OptionValueSpecification.ForVersion()),
                "installs or upgrades chef to the specified version")
        {
        }

        protected override Task<ScheduledTaskStatus> RunCore(IChefServer chefServer, string[] args)
        {
            return chefServer.InstallChef(DownloadChefOption.FindVersion(args));
        }

        protected override string ToDescription(string[] args)
        {
            return $"Installing Chef {DownloadChefOption.FindVersion(args)}";
        }
    }
}