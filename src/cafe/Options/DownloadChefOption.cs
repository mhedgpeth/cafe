using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Options
{
    public class DownloadChefOption : ChefOption
    {
        private readonly ClientFactory _clientFactory;
        private readonly SchedulerWaiter _schedulerWaiter;

        public DownloadChefOption(ClientFactory clientFactory, SchedulerWaiter schedulerWaiter)
            : base(clientFactory, schedulerWaiter,
                "downloads the provided version of chef")
        {
            _clientFactory = clientFactory;
            _schedulerWaiter = schedulerWaiter;
        }

        protected override Task<JobRunStatus> RunCore(IChefServer chefServer, string[] args)
        {
            return chefServer.DownloadChef(FindVersion(args));
        }

        public static string FindVersion(string[] args)
        {
            return args[2];
        }

        protected override string ToDescription(string[] args)
        {
            return $"Downloading Chef {FindVersion(args)}";
        }
    }
}