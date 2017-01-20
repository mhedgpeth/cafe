using cafe.Shared;

namespace cafe.Chef
{
    public interface IInstaller
    {
        Result InstallOrUpgrade(string version, IMessagePresenter presenter);
    }
}