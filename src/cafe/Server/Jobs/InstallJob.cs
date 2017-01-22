using System;
using cafe.Chef;
using cafe.Shared;
using NodaTime;

namespace cafe.Server.Jobs
{
    public class InstallJob : Job
    {
        private readonly IInstaller _installer;
        private readonly IClock _clock;

        public InstallJob(IInstaller installer, IClock clock)
        {
            _installer = installer;
            _clock = clock;
        }

        public Version CurrentVersion => _installer.InstalledVersion;

        public JobRunStatus InstallOrUpgrade(string version)
        {
            return OnRunReady(new JobRun($"Install/Upgrade {_installer.ProductName} to {version}", presenter => _installer.InstallOrUpgrade(version, presenter), _clock));
        }
    }
}