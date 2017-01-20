using cafe.Chef;
using cafe.Shared;
using NodaTime;

namespace cafe.Server.Jobs
{
    public class InstallChefJob : Job
    {
        private readonly IInstaller _installer;
        private readonly IClock _clock;

        public InstallChefJob(IInstaller installer, IClock clock)
        {
            _installer = installer;
            _clock = clock;
        }

        public ScheduledTaskStatus InstallOrUpgrade(string version)
        {
            return OnRunReady(new JobRun($"Install/Upgrade Chef to {version}", presenter => _installer.InstallOrUpgrade(version, presenter), _clock));
        }
    }
}