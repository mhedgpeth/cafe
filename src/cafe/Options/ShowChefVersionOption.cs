using cafe.Client;
using cafe.CommandLine;
using NLog;

namespace cafe.Options
{
    public class ShowChefVersionOption : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ShowChefVersionOption).FullName);

        private readonly ClientFactory _clientFactory;

        public ShowChefVersionOption(ClientFactory clientFactory)
            : base(new OptionSpecification("chef", "version"), "show the current version of chef")
        {
            _clientFactory = clientFactory;
        }

        protected override void RunCore(string[] args)
        {
            var api = _clientFactory.RestClientForChefServer();
            var status = api.GetChefStatus().Result;
            Presenter.ShowMessage($"chef-client version: {status.Version}", Logger);
        }
    }
}