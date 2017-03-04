using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.LocalSystem;
using cafe.Shared;

namespace cafe.Options.Chef
{
    public class BootstrapChefRunListOption : RunJobOption<IChefServer>
    {
        private readonly IFileSystemCommands _fileSystemCommands;

        public BootstrapChefRunListOption(Func<IChefServer> chefServerFactory, ISchedulerWaiter schedulerWaiter,
        IFileSystemCommands fileSystemCommands)
            : base(chefServerFactory, schedulerWaiter,
                "boostraps chef to run the first time with the given policy name and group")
        {
            _fileSystemCommands = fileSystemCommands;
        }

        protected override string ToDescription(Argument[] args)
        {
            return $"Bootstrapping Chef with Run List {FindRunList(args)}";
        }

        private static string FindRunList(IEnumerable<Argument> args)
        {
            return args.FindValueFromLabel("run-list:").Value;
        }

        protected override Task<JobRunStatus> RunJobCore(IChefServer productServer, Argument[] args)
        {

            var config = _fileSystemCommands.ReadAllText(args.FindValueFromLabel("config:").Value);
            var validator = _fileSystemCommands.ReadAllText(args.FindValueFromLabel("validator:").Value);
            return productServer.BootstrapChef(config, validator, FindRunList(args));
        }
    }
}