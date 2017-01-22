using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cafe.Client;
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

        protected override string ToDescription(string[] args)
        {
            return $"Bootstrapping Chef to Policy {FindPolicyName(args)} and Group {FindPolicyGroup(args)}";
        }

        private static string FindPolicyName(IReadOnlyList<string> args)
        {
            return args[3];
        }

        private static string FindPolicyGroup(IReadOnlyList<string> args)
        {
            return args[5];
        }

        protected override Task<JobRunStatus> RunJobCore(IChefServer productServer, string[] args)
        {
            var config = _fileSystemCommands.ReadAllText(args[7]);
            var validator = _fileSystemCommands.ReadAllText(args[9]);
            return productServer.BootstrapChef(config, validator, FindPolicyName(args), FindPolicyGroup(args));
        }
    }
}