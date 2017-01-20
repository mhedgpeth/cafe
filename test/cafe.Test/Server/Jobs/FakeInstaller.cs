using cafe.Chef;
using cafe.Shared;

namespace cafe.Test.Server.Jobs
{
    public class FakeInstaller : IInstaller
    {
        public Result InstallOrUpgrade(string version, IMessagePresenter presenter)
        {
            InstalledVersion = version;
            return Result.Successful();
        }

        public string InstalledVersion { get; private set; }
    }
}