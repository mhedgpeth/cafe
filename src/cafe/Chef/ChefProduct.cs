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

        private readonly string _name;
        private readonly IInstalledProductsFinder _installedProductsFinder;
        private readonly IProductInstaller _installer;
        private readonly Func<ProductInstallationMetaData, bool> _productMatcher;

        public ChefProduct(string name, IInstalledProductsFinder installedProductsFinder, IProductInstaller installer,
            Func<ProductInstallationMetaData, bool> productMatcher)
        {
            _name = name;
            _installedProductsFinder = installedProductsFinder;
            _installer = installer;
            _productMatcher = productMatcher;
        }



        public Version InstalledVersion
        {
            get
            {
                var product = _installedProductsFinder.GetInstalledProducts()
                    .FirstOrDefault(_productMatcher);
                return product == null ? null : Version.Parse(product.DisplayVersion);
            }
        }

        public string ProductName => _name;

        public bool IsInstalled => _installedProductsFinder.GetInstalledProducts().Any(_productMatcher);

        public Result InstallOrUpgrade(string version, IMessagePresenter presenter)
        {
            var productInstallerMetaData = FindProductInstallationMetaData();
            bool isInstalled = productInstallerMetaData != null;
            if (isInstalled)
            {
                presenter.ShowMessage(
                    $"Since {productInstallerMetaData.DisplayName} is already installed, uninstalling it");
                var uninstallResult = _installer.Uninstall(ProductCode);
                if (uninstallResult.IsFailed)
                {
                    presenter.ShowMessage($"Uninstall failed, so {_name} cannot be installed on this machine");
                    return uninstallResult;
                }
                if (IsInstalled)
                {
                    presenter.ShowMessage(
                        $"{_name} is still installed on this machine after the uninstall, so we cannot install");
                    return Result.Failure($"Uninstall of {_name} failed");
                }
            }
            else
            {
                presenter.ShowMessage($"{_name} is not installed, so installing it for the first time");
            }
            presenter.ShowMessage($"Installing {_name} {version}");
            var result = _installer.Install(version);
            if (result.IsFailed)
            {
                Logger.Info($"Could not install {_name} {version} because of an error: {result}");
                presenter.ShowMessage($"Could not install {_name} {version}. Make sure your server is running as an administrator");
            }
            else if (!IsInstalled)
            {
                presenter.ShowMessage($"Installer for {_name} {version} ran successfully, but it is still not installed. Make sure your server has rights to install {_name} for all users (Administrator rights).");
                return Result.Failure(
                    $"Expecting {_name} to be installed after running the installer, but it is not installed");
            }
            Logger.Info($"Result of install {_name} {version} is {result}");
            return result;
        }

        private ProductInstallationMetaData FindProductInstallationMetaData()
        {
            var installedProducts = _installedProductsFinder.GetInstalledProducts();
            var productInstallerMetaData = installedProducts
                .FirstOrDefault(_productMatcher);
            return productInstallerMetaData;
        }

        public string ProductCode
        {
            get
            {
                var product = _installedProductsFinder.GetInstalledProducts()
                    .FirstOrDefault(_productMatcher);
                return product?.Parent;

            }
        }
    }
}