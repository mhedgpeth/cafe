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

        public StatusOption(Func<IChefServer> schedulerServerProvider) : base(schedulerServerProvider,
            new OptionSpecification("status"),
            "Gets the status of the cafe server")
        {
        }

        protected override Result RunCore(IChefServer schedulerServer, string[] args)
        {
            var status = schedulerServer.GetStatus().Result;
            ShowQueuedTasks(status);
            ShowFinishedTasks(status);
            ShowChefStatus(status);
            return Result.Successful();
        }

        private void ShowChefStatus(ServerStatus status)
        {
            Presenter.NewLine();
            Presenter.ShowMessage(status.ChefStatus.ToString(), Logger);
        }

        private static void ShowQueuedTasks(ServerStatus status)
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

        private static void ShowFinishedTasks(ServerStatus status)
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