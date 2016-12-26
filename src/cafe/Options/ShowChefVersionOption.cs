using cafe.Client;
using cafe.CommandLine;
using Microsoft.Extensions.Logging;

namespace cafe.Options
{
    public class ShowChefVersionOption : Option
    {
        private readonly ClientFactory _clientFactory;

        private static ILogger Logger { get; } =
          ApplicationLogging.CreateLogger<ShowChefVersionOption>();

        public ShowChefVersionOption(ClientFactory clientFactory)
        : base(new OptionSpecification("chef", "version"), "show the current version of chef")
        {
            _clientFactory = clientFactory;
        }

        protected override void RunCore(string[] args)
        {
            var api = _clientFactory.RestClientForChefServer();
            var status = api.GetChefStatus().Result;
            Logger.LogInformation($"chef-client version: {status.Version}");
        }
    }
}