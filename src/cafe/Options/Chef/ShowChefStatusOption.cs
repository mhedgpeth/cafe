using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options.Chef
{
    public class ShowChefStatusOption : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ShowChefStatusOption).FullName);

        private readonly ClientFactory _clientFactory;

        public ShowChefStatusOption(ClientFactory clientFactory)
            : base("show the status of chef")
        {
            _clientFactory = clientFactory;
        }

        protected override Result RunCore(string[] args)
        {
            var api = _clientFactory.RestClientForChefServer();
            var status = api.GetStatus().Result;
            Presenter.ShowMessage("Chef Status:", Logger);
            Presenter.ShowMessage(status.ToString(), Logger);
            var versionStatus = status.Version ?? "not installed";
            Presenter.ShowMessage($"chef-client version: {versionStatus}", Logger);
            Presenter.ShowMessage($"Last run: {status.LastRun?.ToLocalTime()}", Logger);
            Presenter.ShowMessage($"Expected Next Run: {status.ExpectedNextRun?.ToLocalTime()}", Logger);
            return Result.Successful();
        }

        protected override string ToDescription(string[] args)
        {
            return $"Determining Chef Status";
        }
    }
}