using cafe.CommandLine;
using cafe.Shared;
using DasMulli.Win32.ServiceUtils;

namespace cafe.Options.Server
{
    public class ServerWindowsServiceOption : Option
    {
        public ServerWindowsServiceOption() : base("Runs cafe in service mode")
        {
        }

        protected override string ToDescription(Argument[] args)
        {
            return "Starting Cafe in Service Mode as a Windows Service";
        }

        protected override Result RunCore(Argument[] args)
        {
            var cafeServerWindowsService = new CafeServerWindowsService();
            var serviceHost = new Win32ServiceHost(cafeServerWindowsService);
            serviceHost.Run();
            return Result.Successful();
        }
    }
}