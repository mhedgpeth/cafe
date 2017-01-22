using System.Collections.Generic;
using Microsoft.Win32;

namespace cafe.LocalSystem
{
    public interface IInstalledProductsFinder
    {
        IEnumerable<ProductInstallationMetaData> GetInstalledProducts();
    }

    public class InstalledProductsFinder : IInstalledProductsFinder
    {
        public static bool IsChefClient(ProductInstallationMetaData metaData)
        {
            return IsPublishedByChefAndNameStartsWith(metaData, "Chef Client");
        }

        private static bool IsPublishedByChefAndNameStartsWith(ProductInstallationMetaData metaData, string name)
        {
            return IsPublishedByChef(metaData) &&
                   metaData.DisplayName.StartsWith(name);
        }

        private static bool IsPublishedByChef(ProductInstallationMetaData metaData)
        {
            return !string.IsNullOrEmpty(metaData.Publisher) && metaData.Publisher.Contains("Chef Software, Inc");
        }

        public static bool IsInspec(ProductInstallationMetaData metaData)
        {
            var isInspec = IsPublishedByChefAndNameStartsWith(metaData, "InSpec");
            return isInspec;
        }

        public IEnumerable<ProductInstallationMetaData> GetInstalledProducts()
        {
            var installedProducts = new List<ProductInstallationMetaData>();
            const string keyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (var registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                .OpenSubKey(keyPath))
            {
                foreach (var subkeyName in registryKey.GetSubKeyNames())
                {
                    using (RegistryKey subkey = registryKey.OpenSubKey(subkeyName))
                    {
                        var name = (string) subkey.GetValue("DisplayName");
                        var publisher = (string) subkey.GetValue("Publisher");
                        var displayVersion = (string) subkey.GetValue("DisplayVersion");
                        var uninstallString = (string) subkey.GetValue("UninstallString");
                        if (!string.IsNullOrEmpty(name))
                        {
                            var metaData = new ProductInstallationMetaData()
                            {
                                DisplayName = name,
                                Publisher = publisher,
                                DisplayVersion = displayVersion,
                                UninstallString = uninstallString,
                                Parent = subkeyName
                            };
                            installedProducts.Add(metaData);
                        }
                    }
                }
            }
            return installedProducts;
        }
    }
}