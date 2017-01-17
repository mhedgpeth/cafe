using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
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

        protected override Result RunCore(string[] args)
        {
            var api = _clientFactory.RestClientForChefServer();
            var status = api.GetChefStatus().Result;
            var versionStatus = status.Version ?? "not installed";
            Presenter.ShowMessage($"chef-client version: {versionStatus}", Logger);
            return Result.Successful();
        }

        protected override string ToDescription(string[] args)
        {
            return $"Determining Chef Version";
        }
    }
}