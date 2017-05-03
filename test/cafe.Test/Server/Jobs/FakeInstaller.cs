using System;
using cafe.Chef;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Test.Server.Jobs
{
    public class FakeInstaller : IInstaller
    {
        public string ProductName => "Fake product";

        public Result InstallOrUpgrade(string version, IMessagePresenter presenter)
        {
            InstalledVersion = Version.Parse(version);
            return Result.Successful();
        }

        public Version InstalledVersion { get; private set; }
    }
}