using System;
using cafe.CommandLine;
using cafe.Shared;
using NLog;
using NodaTime;

namespace cafe.Server.Jobs
{
    public class JobRun : IMessagePresenter, IJobRun
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(JobRun).FullName);

        private readonly string _description;
        private readonly Func<IMessagePresenter, Result> _action;
        private readonly IClock _clock;
        private JobRunState _state = JobRunState.NotRun;
        private readonly Guid _id = Guid.NewGuid();
        private Instant? _start;
        private Instant? _finish;
        private string _currentMessage;
        private Result _result;

        public JobRun(string description, Func<IMessagePresenter, Result> action, IClock clock)
        {
            _description = description;
            _action = action;
            _clock = clock;
        }

        public bool IsRunning => _state == JobRunState.Running;
        public bool IsFinishedRunning => _state == JobRunState.Finished;
        public Guid Id => _id;
        public Instant? Start => _start;
        public Instant? Finish => _finish;
        public string CurrentMessage => _currentMessage;
        public Result Result => _result;

        public void Run()
        {
            StartRun();
            _result = RunCore();
            FinishRun();
        }

        private Result RunCore()
        {
            Result result;
            try
            {
                result = _action(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex,
                    $"An unexpected error occurred while runnnig {_description} ({Id}): {ex.Message}");
                result = Result.Failure($"An unexpected error occurred: {ex.Message}");
            }
            return result;
        }

        private void FinishRun()
        {
            _finish = _clock.GetCurrentInstant();
            _state = JobRunState.Finished;
            ShowMessage(
                $"Job Run {_description} ({Id}) finished at {_finish} with result: {Result}");

        }

        private void StartRun()
        {
            _start = _clock.GetCurrentInstant();
            _state = JobRunState.Running;
            ShowMessage($"Job Run {_description} ({Id}) started at {Start}");
        }

        public void ShowMessage(string message)
        {
            Logger.Info(message);
            _currentMessage = message;
        }

        public override string ToString()
        {
            return ToStatus().ToString();
        }

        public JobRunStatus ToStatus()
        {
            return new JobRunStatus
            {
                Id = _id,
                State = _state,
                CurrentMessage = _currentMessage,
                Description = _description,
                Result = _result,
                StartTime = Start?.ToDateTimeUtc(),
                FinishTime = Finish?.ToDateTimeUtc()
            };
        }
    }
}