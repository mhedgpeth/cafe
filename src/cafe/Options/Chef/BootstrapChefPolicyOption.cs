using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.CommandLine.LocalSystem;
using cafe.LocalSystem;
using cafe.Shared;

namespace cafe.Options.Chef
{
    public class BootstrapChefPolicyOption : RunJobOption<IChefServer>
    {
        private readonly IFileSystemCommands _fileSystemCommands;

        public BootstrapChefPolicyOption(Func<IChefServer> chefServerFactory, ISchedulerWaiter schedulerWaiter,
            IFileSystemCommands fileSystemCommands)
            : base(chefServerFactory, schedulerWaiter,
                "boostraps chef to run the first time with the given policy name and group")
        {
            _fileSystemCommands = fileSystemCommands;
        }

        protected override string ToDescription(Argument[] args)
        {
            return $"Bootstrapping Chef to Policy {FindPolicyValue(args)} and Group {FindGroupValue(args)}";
        }

        private static string FindGroupValue(Argument[] args)
        {
            return args.FindValueFromLabel("group:").Value;
        }

        private static string FindPolicyValue(Argument[] args)
        {
            return args.FindValueFromLabel("policy:").Value;
        }


        protected override Task<JobRunStatus> RunJobCore(IChefServer productServer, Argument[] args)
        {
            var config = _fileSystemCommands.ReadAllText(args.FindValueFromLabel("config:").Value);
            var validator = _fileSystemCommands.ReadAllText(args.FindValueFromLabel("validator:").Value);
            return productServer.BootstrapChef(config, validator, FindPolicyValue(args), FindGroupValue(args));
        }
    }
}