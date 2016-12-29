using Microsoft.Win32;
using NLog;

namespace cafe.LocalSystem
{
    public class EnvironmentBoundary : IEnvironment
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(EnvironmentBoundary).FullName);

        public string GetEnvironmentVariable(string key)
        {
            var value = System.Environment.GetEnvironmentVariable(key);
            Logger.Debug($"Retrieved environment variable {key} with value: {value}");
            return value;
        }

        public void SetSystemEnvironmentVariable(string key, string value)
        {
            const string keyPath = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
            using (var registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                .OpenSubKey(keyPath, true))
            {
                registryKey.SetValue("Path", value, RegistryValueKind.ExpandString);
            }
        }
    }
}