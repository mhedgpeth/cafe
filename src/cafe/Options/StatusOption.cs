using System;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options
{
    public class StatusOption : Option
    {
        private readonly Func<ISchedulerServer> _schedulerServerProvider;
        private static readonly Logger Logger = LogManager.GetLogger(typeof(StatusOption).FullName);

        public StatusOption(Func<ISchedulerServer> schedulerServerProvider) : base(
            new OptionSpecification("status"),
            "Gets the status of the cafe server")
        {
            _schedulerServerProvider = schedulerServerProvider;
        }

        protected override Result RunCore(string[] args)
        {
            try
            {
                var status = _schedulerServerProvider().GetStatus().Result;
                Presenter.ShowMessage($"Is Running: {status.IsRunning}", Logger);
                Presenter.NewLine();
                if (status.QueuedTasks.Length > 0)
                {
                    Presenter.ShowMessage($"Queued tasks ({status.QueuedTasks.Length}):", Logger);
                    foreach (var queuedTask in status.QueuedTasks)
                    {
                        Presenter.ShowMessage(queuedTask.ToString(), Logger);
                    }
                }
                else
                {
                    Presenter.ShowMessage("There are currently no queued tasks", Logger);
                }
                Presenter.NewLine();
                var count = 0;
                if (status.FinishedTasks.Length > 0)
                {
                    Presenter.ShowMessage("Finished tasks (latest 10):", Logger);
                    foreach (var finishedTask in status.FinishedTasks)
                    {
                        count++;
                        Presenter.ShowMessage(finishedTask.ToString(), Logger);
                        if (count > 10)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    Presenter.ShowMessage("There are currently no finished tasks", Logger);
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "An exception occurred while trying to get the status of the server");
                Presenter.ShowMessage("The server is not currently running", Logger);
            }
            return Result.Successful();
        }

        protected override string ToDescription(string[] args)
        {
            return "Determining Status";
        }
    }
}