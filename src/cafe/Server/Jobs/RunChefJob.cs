using System;
using cafe.Chef;
using cafe.Shared;
using NodaTime;

namespace cafe.Server.Jobs
{
    public class RunChefJob : Job
    {
        private readonly IChefRunner _chefRunner;
        private readonly IClock _clock;
        private RunPolicy _runPolicy;

        public RunChefJob(RunPolicy runPolicy, IChefRunner chefRunner, IClock clock)
        {
            _chefRunner = chefRunner;
            _clock = clock;
            RunPolicy = runPolicy;
        }

        public RunPolicy RunPolicy
        {
            get { return _runPolicy; }
            set
            {
                value.Due += ProcessPolicyDue;
                _runPolicy = value;
            }
        }

        public bool IsRunning { get; private set; } = true;

        private void ProcessPolicyDue(object sender, EventArgs args)
        {
            if (IsRunning && (LastRun == null || LastRun.IsFinishedRunning))
            {
                Run();
            }
        }

        public ScheduledTaskStatus Run()
        {
            return OnRunReady(new JobRun("Run Chef", messagePresenter  => _chefRunner.Run(messagePresenter), _clock));
        }

        public ScheduledTaskStatus Bootstrap(IChefBootstrapper bootstrapper)
        {
            return OnRunReady(new JobRun($"Bootstrapping Chef with {bootstrapper}", messagePresenter => _chefRunner.Run(messagePresenter, bootstrapper), _clock));
        }

        public RecurringTaskStatus Pause()
        {
            IsRunning = false;
            return ToStatus();
        }

        public RecurringTaskStatus Resume()
        {
            IsRunning = true;
            return ToStatus();
        }

        public RecurringTaskStatus ToStatus()
        {
            return new RecurringTaskStatus()
            {
                IsRunning = IsRunning,
                LastRun = LastRun?.Finish?.ToDateTimeUtc(),
            };
        }

    }
}