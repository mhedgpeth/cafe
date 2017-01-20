using System;
using cafe.Shared;

namespace cafe.Chef
{
    public interface IInstaller
    {
        Version InstalledVersion { get; }
        Result InstallOrUpgrade(string version, IMessagePresenter presenter);
    }
}