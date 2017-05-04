using cafe.Chef;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Test.Chef
{
    public class FakeProductInstaller : IProductInstaller
    {
        private readonly FakeInstalledProductsFinder _installedProductsFinder;

        public FakeProductInstaller(FakeInstalledProductsFinder installedProductsFinder)
        {
            _installedProductsFinder = installedProductsFinder;
        }

        public Result Uninstall(string productCode)
        {
            _installedProductsFinder.InstalledProducts.Clear();
            ProductCodeUninstalled = productCode;
            return Result.Successful();
        }

        public string ProductCodeUninstalled { get; private set; }

        public Result Install(string version)
        {
            VersionInstalled = version;
            _installedProductsFinder.InstalledProducts.Add(ChefProductTest.CreateProductInstallationMetaData(version));
            return Result.Successful();
        }

        public string VersionInstalled { get; private set; }
    }
}