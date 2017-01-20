using System;
using System.Linq;
using cafe.LocalSystem;
using cafe.Shared;
using NLog;

namespace cafe.Chef
{
    public class ChefProduct : IInstaller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefProduct).FullName);

        private readonly IInstalledProductsFinder _installedProductsFinder;
        private readonly IProductInstaller _installer;

        public ChefProduct(IInstalledProductsFinder installedProductsFinder, IProductInstaller installer)
        {
            _installedProductsFinder = installedProductsFinder;
            _installer = installer;
        }

        public Version InstalledVersion
        {
            get
            {
                var chef = _installedProductsFinder.GetInstalledProducts()
                    .FirstOrDefault(InstalledProductsFinder.IsChefClient);
                return chef == null ? null : Version.Parse(chef.DisplayVersion);
            }
        }

        public bool IsInstalled => _installedProductsFinder.GetInstalledProducts().Any(InstalledProductsFinder.IsChefClient);

        public ChefStatus ToChefStatus()
        {
            var status = new ChefStatus();
            var installedVersion = InstalledVersion;
            if (installedVersion != null)
            {
                status.Version = installedVersion.ToString();
            }
            return status;
        }

        public Result InstallOrUpgrade(string version, IMessagePresenter presenter)
        {
            var chefInstallationMetaData = FindChefInstallationMetaData();
            bool isInstalled = chefInstallationMetaData != null;
            if (isInstalled)
            {
                presenter.ShowMessage($"Since {chefInstallationMetaData.DisplayName} is already installed, uninstalling it");
                var uninstallResult = _installer.Uninstall(ProductCode);
                if (uninstallResult.IsFailed)
                {
                    presenter.ShowMessage("Uninstall failed, so chef cannot be installed on this machine");
                    return uninstallResult;
                }
                if (IsInstalled)
                {
                    presenter.ShowMessage("Chef is still installed on this machine after the uninstall, so we cannot install");
                    return Result.Failure("Uninstall of chef client failed");
                }
            }
            presenter.ShowMessage($"Installing Chef {version}");
            var result = _installer.Install(version);
            if (result.IsFailed)
            {
                Logger.Info($"Could not install chef {version} because of an error: {result}");
                presenter.ShowMessage("Could not install Chef {version}. Make sure your server is running as an administrator");
            }
            else if (!IsInstalled)
            {
                presenter.ShowMessage($"Installer for Chef {version} ran successfully, but it is still not installed. Make sure your server has rights to install chef for all users (Administrator rights).");
                return Result.Failure(
                    $"Expecting chef to be installed after running the installer, but it is not installed");
            }
            Logger.Info($"Result of install Chef {version} is {result}");
            return result;
        }

        private ProductInstallationMetaData FindChefInstallationMetaData()
        {
            var chefInstallationMetaData = _installedProductsFinder.GetInstalledProducts()
                .FirstOrDefault(InstalledProductsFinder.IsChefClient);
            return chefInstallationMetaData;
        }

        public string ProductCode
        {
            get
            {
                var chef = _installedProductsFinder.GetInstalledProducts()
                    .FirstOrDefault(InstalledProductsFinder.IsChefClient);
                return chef?.Parent;

            }
        }
    }
}