using cafe.Chef;
using cafe.LocalSystem;
using FluentAssertions;
using Xunit;

namespace cafe.Test.Chef
{
    public class ChefProductTest
    {
        [Fact]
        public void InstalledVersion_ShouldBeNullWhenChefIsNotInstalled()
        {
            var product = CreateChefProduct(new FakeInstalledProductsFinder());

            product.InstalledVersion.Should().BeNull("because there isn't a matching chef product");
        }

        [Fact]
        public void InstalledVersion_ShouldBeMatchingChefVersionWhenItExists()
        {
            var product = CreateChefProduct(CreateProductsFinderWithChefInstalled());

            product.InstalledVersion.Should().Be(new System.Version(12, 17, 44, 1));
        }

        private FakeInstalledProductsFinder CreateProductsFinderWithChefInstalled()
        {
            var finder = new FakeInstalledProductsFinder();
            finder.InstalledProducts.Add(CreateProductInstallationMetaData("12.17.44"));
            return finder;
        }

        public static ProductInstallationMetaData CreateProductInstallationMetaData(string version)
        {
            return new ProductInstallationMetaData()
            {
                DisplayName = $"Chef Client v{version}",
                DisplayVersion = $"{version}.1",
                Publisher = "Chef Software, Inc.",
                UninstallString = "MsiExec.exe /I{7FC5FAC2-7AFE-4AE6-9548-DFE6F69B325D}"
            };
        }

        [Fact]
        public void IsInstalled_ShouldBeTrueWhenChefProductExists()
        {
            CreateChefProduct(CreateProductsFinderWithChefInstalled()).IsInstalled.Should()
                .BeTrue("because a chef product is found in the list of installed products");
        }

        private ChefProduct CreateChefProduct(FakeInstalledProductsFinder finder)
        {
            var fakeProductInstaller = new FakeProductInstaller(finder);
            return CreateChefProduct(finder, fakeProductInstaller);
        }

        private static ChefProduct CreateChefProduct(FakeInstalledProductsFinder finder, FakeProductInstaller fakeProductInstaller)
        {
            return new ChefProduct("chef", finder, fakeProductInstaller, InstalledProductsFinder.IsChefClient);
        }

        [Fact]
        public void IsInstalled_ShouldBeFalseWhenChefProductDoesNotExist()
        {
            CreateChefProduct(new FakeInstalledProductsFinder()).IsInstalled.Should()
                .BeFalse("because a chef product isn't found in the list of installed products");
        }

        [Fact]
        public void InstallOrUpgrade_ShouldUninstallExistingProductThenInstallNewOne()
        {
            var chefInstallationMetaData = CreateProductInstallationMetaData("1.2.3");
            var finder = new FakeInstalledProductsFinder(chefInstallationMetaData);
            var installer = new FakeProductInstaller(finder);

            var product = new ChefProduct("chef", finder, installer, InstalledProductsFinder.IsChefClient);

            const string versionInstalled = "15.1.2";
            product.InstallOrUpgrade(versionInstalled, new FakeMessagePresenter());

            installer.ProductCodeUninstalled.Should()
                .Be(chefInstallationMetaData.Parent,
                    "because since the product is already installed, we should uninstall it");
            installer.VersionInstalled.Should().Be(versionInstalled);
        }

        [Fact]
        public void InstallOrUpgrade_ShouldFailWhenNotStaged()
        {
            var finder = CreateProductsFinderWithChefInstalled();
            var fakeProductInstaller = new FakeProductInstaller(finder);
            fakeProductInstaller.IsStagedValue = false;
            var product = CreateChefProduct(finder, fakeProductInstaller);

            var result = product.InstallOrUpgrade("15.1.2", new FakeMessagePresenter());

            result.IsFailed.Should().BeTrue("because the product isn't staged");
        }
    }
}