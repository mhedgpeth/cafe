using System.Collections.Generic;
using System.Threading.Tasks;
using cafe.Client;
using cafe.LocalSystem;
using cafe.Shared;

namespace cafe.Options
{
    public class BootstrapChefRunListOption : ChefOption
    {
        private readonly IFileSystemCommands _fileSystemCommands;

        public BootstrapChefRunListOption(IClientFactory clientFactory, ISchedulerWaiter schedulerWaiter,
        IFileSystemCommands fileSystemCommands)
            : base(clientFactory, schedulerWaiter,
                "boostraps chef to run the first time with the given policy name and group")
        {
            _fileSystemCommands = fileSystemCommands;
        }

        protected override string ToDescription(string[] args)
        {
            return $"Bootstrapping Chef with Run List {FindRunList(args)}";
        }

        private static string FindRunList(IReadOnlyList<string> args)
        {
            return args[3];
        }

        protected override Task<JobRunStatus> RunCore(IChefServer chefServer, string[] args)
        {

            var config = _fileSystemCommands.ReadAllText(args[5]);
            var validator = _fileSystemCommands.ReadAllText(args[7]);
            return chefServer.BootstrapChef(config, validator, FindRunList(args));
        }
    }
}