using System;
using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options.Chef
{
    public abstract class RunJobOption<T> : ServerConnectionOption<T>
    {
        private static readonly Logger Logger = LogManager.GetLogger("cafe.Options.Chef.RunJobOption");

        private readonly ISchedulerWaiter _schedulerWaiter;

        protected RunJobOption(Func<T> serverFactory, ISchedulerWaiter schedulerWaiter, string helpText)
            : base(serverFactory, helpText)
        {
            _schedulerWaiter = schedulerWaiter;
        }

        protected sealed override Result RunCore(T client, Argument[] args)
        {
            var status = RunJobCore(client, args).Result;
            if (!args.HasArgumentLabeled("return:") || args.FindValueFromLabel("return:").Value != "immediately")
            {
                var finalStatus = _schedulerWaiter.WaitForTaskToComplete(status);
                Logger.Info($"Finished running {finalStatus.Description}");
                return finalStatus.Result;
            }
            else
            {
                Logger.Info("This job is configured not to wait for a response, so returning core status");
                return status.Result;
            }
        }

        protected abstract Task<JobRunStatus> RunJobCore(T productServer, Argument[] args);
    }
}