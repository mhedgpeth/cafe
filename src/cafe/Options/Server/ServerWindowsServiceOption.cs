using cafe.CommandLine;
using cafe.Shared;
using DasMulli.Win32.ServiceUtils;

namespace cafe.Options.Server
{
    public class ServerWindowsServiceOption : Option
    {
        public ServerWindowsServiceOption() : base(new OptionSpecification("server", "--run-as-service"), "Runs cafe in service mode")
        {
        }

        protected override string ToDescription(string[] args)
        {
            return "Starting Cafe in Service Mode as a Windows Service";
        }

        protected override Result RunCore(string[] args)
        {
            var cafeServerWindowsService = new CafeServerWindowsService();
            var serviceHost = new Win32ServiceHost(cafeServerWindowsService);
            serviceHost.Run();
            return Result.Successful();
        }
    }
}