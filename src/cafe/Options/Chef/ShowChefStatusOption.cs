using System;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options.Chef
{
    public class ShowChefStatusOption : ServerConnectionOption<IChefServer>
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ShowChefStatusOption).FullName);


        public ShowChefStatusOption(Func<IChefServer> serverFactory)
            : base(serverFactory, "show the status of chef")
        {
        }

        protected override Result RunCore(IChefServer client, Argument[] args)
        {
            var status = client.GetStatus().Result;
            ShowProductStatus(status, "Chef");
            Presenter.ShowMessage($"Last run: {status.LastRun?.ToLocalTime()}", Logger);
            Presenter.ShowMessage($"Expected Next Run: {status.ExpectedNextRun?.ToLocalTime()}", Logger);
            return Result.Successful();
        }

        public static void ShowProductStatus(ProductStatus status, string productName)
        {
            Presenter.ShowMessage($"{productName} Status:", Logger);
            Presenter.ShowMessage(status.ToString(), Logger);
            var versionStatus = status.Version ?? "not installed";
            Presenter.ShowMessage($"{productName} version: {versionStatus}", Logger);
        }

        protected override string ToDescription(Argument[] args)
        {
            return $"Determining Chef Status";
        }
    }
}