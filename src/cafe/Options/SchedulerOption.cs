using System;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options
{
    public abstract class SchedulerOption : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(SchedulerOption).FullName);
        private readonly Func<ISchedulerServer> _schedulerServerProvider;

        protected SchedulerOption(Func<ISchedulerServer> schedulerServerProvider,
            OptionSpecification optionSpecification,
            string helpText)
            : base(optionSpecification, helpText)
        {
            _schedulerServerProvider = schedulerServerProvider;
        }

        protected sealed override Result RunCore(string[] args)
        {
            try
            {
                var schedulerServer = _schedulerServerProvider();
                RunCore(schedulerServer, args);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, $"An exception occurred while {ToDescription(args)}");
                Presenter.ShowMessage("The server is not currently running", Logger);
            }
            return Result.Successful();
        }

        protected abstract Result RunCore(ISchedulerServer server, string[] args);
    }
}