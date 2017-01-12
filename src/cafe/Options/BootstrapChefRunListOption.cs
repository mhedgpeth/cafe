using System.Collections.Generic;
using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
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
                new OptionSpecification(OptionValueSpecification.ForExactValue("chef"),
                    OptionValueSpecification.ForExactValue("bootstrap"),
                    OptionValueSpecification.ForExactValue("run-list:"),
                    OptionValueSpecification.ForAnyValue("the run list"),
                    OptionValueSpecification.ForExactValue("config:"),
                    OptionValueSpecification.ForAnyValue("the client.rb file"),
                    OptionValueSpecification.ForExactValue("validator:"),
                    OptionValueSpecification.ForAnyValue("the validator.pem file used to join the node")),
                "boostraps chef to run the first time with the given policy name and group")
        {
            _fileSystemCommands = fileSystemCommands;
        }

        protected override string ToDescription(string[] args)
        {
            return $"Bootstrapping Chef with Run List #{FindRunList(args)}";
        }

        private static string FindRunList(IReadOnlyList<string> args)
        {
            return args[3];
        }

        protected override Task<ScheduledTaskStatus> RunCore(IChefServer chefServer, string[] args)
        {
            var config = _fileSystemCommands.ReadAllText(args[5]);
            var validator = _fileSystemCommands.ReadAllText(args[7]);
            return chefServer.BootstrapChef(config, validator, FindRunList(args));
        }
    }
}