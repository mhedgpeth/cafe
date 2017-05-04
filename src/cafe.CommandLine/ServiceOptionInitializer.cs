using cafe.CommandLine.LocalSystem;
using cafe.CommandLine.Options;

namespace cafe.CommandLine
{
    public static class ServiceOptionInitializer
    {
        public static void AddServiceOptionsTo(OptionGroup serviceGroup, ServiceStatusWaiter serviceStatusWaiter, ProcessExecutor processExecutor, IFileSystem fileSystem,
            string serviceName, string serviceDisplayName, string serviceDescription)
        {
            serviceGroup.WithOption(new RegisterServerWindowsServiceOption(serviceName, serviceDisplayName, serviceDescription), "register");
            serviceGroup.WithOption(new UnregisterServerWindowsServiceOption(serviceName, serviceDisplayName), "unregister");
            serviceGroup.WithOption(ChangeStateForCafeWindowsServiceOption.StartCafeWindowsServiceOption(
                processExecutor, fileSystem,
                serviceStatusWaiter, serviceName), "start");
            serviceGroup.WithOption(ChangeStateForCafeWindowsServiceOption.StopCafeWindowsServiceOption(
                processExecutor, fileSystem, serviceStatusWaiter, serviceName), "stop");
            serviceGroup.WithOption(new CafeWindowsServiceStatusOption(processExecutor, fileSystem, serviceName), "status");
        }
    }
}