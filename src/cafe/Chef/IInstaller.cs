using System;
using cafe.Shared;

namespace cafe.Chef
{
    public interface IInstaller
    {
        Version InstalledVersion { get; }
        string ProductName { get; }
        Result InstallOrUpgrade(string version, IMessagePresenter presenter);
    }
}