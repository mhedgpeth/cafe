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
            return string.Equals("Chef Software, Inc.", metaData.Publisher) &&
                   metaData.DisplayName.StartsWith("Chef Client");
        }

        public IEnumerable<ProductInstallationMetaData> GetInstalledProducts()
        {
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
                            yield return metaData;
                        }
                    }
                }
            }
        }

    }
}