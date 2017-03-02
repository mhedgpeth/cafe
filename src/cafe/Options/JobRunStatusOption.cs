using System;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options
{
    public class JobRunStatusOption : ServerConnectionOption<IJobServer>
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(JobRunStatusOption).FullName);

        public JobRunStatusOption(Func<IJobServer> schedulerServerProvider)
            : base(schedulerServerProvider, "Gets the id of the job")
        {
        }

        protected override string ToDescription(Argument[] args)
        {
            return $"Retrieving Status for Job {IdArgument(args)}";
        }

        protected override Result RunCore(IJobServer server, Argument[] args)
        {
            Guid id;
            var idArgument = IdArgument(args);
            bool success = Guid.TryParse(idArgument, out id);
            if (success)
            {
                var status = server.GetJobRunStatus(id).Result;
                Presenter.ShowMessage($"Job Run status for {status.Description} ({id}):", Logger);
                Presenter.ShowMessage($"State: {status.State}", Logger);
                Presenter.ShowMessage($"Started: {status.StartTime?.ToLocalTime()}", Logger);
                Presenter.ShowMessage($"Finished: {status.FinishTime?.ToLocalTime()}", Logger);
                Presenter.ShowMessage($"Duration (seconds): {Convert.ToInt32(status.Duration?.TotalSeconds)}", Logger);
                Presenter.ShowMessage($"Result: {status.Result}", Logger);
                Presenter.ShowMessage($"Current Message: {status.CurrentMessage}", Logger);
                return Result.Successful();
            }
            else
            {
                return Result.Failure($"Could not parse {idArgument} as a job run id");
            }
        }

        private static string IdArgument(Argument[] args)
        {
            return args.FindValueFromLabel("id:").Value;
        }
    }
}