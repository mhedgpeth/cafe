using System;
using System.Threading.Tasks;
using cafe.Client;
using cafe.Shared;
using NLog;

namespace cafe.Options
{
    public abstract class ChefJobOption : ServerConnectionOption<IChefServer>
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefJobOption).FullName);

        private readonly Func<IChefServer> _chefServerFactory;
        private readonly ISchedulerWaiter _schedulerWaiter;

        protected ChefJobOption(Func<IChefServer> chefServerFactory, ISchedulerWaiter schedulerWaiter, string helpText)
            : base(chefServerFactory, helpText)
        {
            _chefServerFactory = chefServerFactory;
            _schedulerWaiter = schedulerWaiter;
        }

        protected sealed override Result RunCore(IChefServer client, string[] args)
        {
            var status = RunJobCore(client, args).Result;
            var finalStatus = _schedulerWaiter.WaitForTaskToComplete(status);
            Logger.Info($"Finished running {finalStatus.Description}");
            return finalStatus.Result;
        }

        protected abstract Task<JobRunStatus> RunJobCore(IChefServer chefServer, string[] args);
    }
}