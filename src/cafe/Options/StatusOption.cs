using System;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options
{
    public class StatusOption : SchedulerOption
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(StatusOption).FullName);

        public StatusOption(Func<ISchedulerServer> schedulerServerProvider) : base(schedulerServerProvider,
            new OptionSpecification("status"),
            "Gets the status of the cafe server")
        {
        }

        protected override Result RunCore(ISchedulerServer schedulerServer, string[] args)
        {
            var status = schedulerServer.GetStatus().Result;
            Presenter.ShowMessage($"Is Running: {status.IsRunning}", Logger);
            ShowQueuedTasks(status);
            ShowFinishedTasks(status);
            ShowRecurringTasks(status);
            return Result.Successful();
        }

        private void ShowRecurringTasks(SchedulerStatus status)
        {
            Presenter.NewLine();
            if (status.RecurringTasks.Length > 0)
            {
                Presenter.ShowMessage($"Recurring tasks ({status.RecurringTasks.Length}):", Logger);
                foreach (var recurringTask in status.RecurringTasks)
                {
                    Presenter.ShowMessage(recurringTask.ToString(), Logger);
                }
            }
            else
            {
                Presenter.ShowMessage("There are no recurring tasks", Logger);
            }
        }

        private static void ShowQueuedTasks(SchedulerStatus status)
        {
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
        }

        private static void ShowFinishedTasks(SchedulerStatus status)
        {
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

        protected override string ToDescription(string[] args)
        {
            return "Determining Status";
        }
    }
}