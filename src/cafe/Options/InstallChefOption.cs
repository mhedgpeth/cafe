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
                    OptionValueSpecification.ForAnyValues("install", "upgrade"), OptionValueSpecification.ForVersion()),
                "installs or upgrades chef to the specified version")
        {
        }

        protected override Task<ScheduledTaskStatus> RunCore(IChefServer chefServer, string[] args)
        {
            return chefServer.InstallChef(args[2]);
        }
    }
}