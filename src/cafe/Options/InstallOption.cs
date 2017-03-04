using System;
using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.Options.Chef;
using cafe.Shared;

namespace cafe.Options
{
    public class InstallOption<TProductServerType, TStatusType> : RunJobOption<TProductServerType> where TProductServerType : IProductServer<TStatusType> where TStatusType : ProductStatus
    {
        private readonly string _product;

        public InstallOption(string product, Func<TProductServerType> productServerCreator, ISchedulerWaiter schedulerWaiter)
            : base(productServerCreator, schedulerWaiter,
                "installs or upgrades chef to the specified version")
        {
            _product = product;
        }

        protected override Task<JobRunStatus> RunJobCore(TProductServerType productServer, Argument[] args)
        {
            return productServer.Install(DownloadProductOption<TProductServerType, TStatusType>.FindVersion(args));
        }

        protected override string ToDescription(Argument[] args)
        {
            return $"Installing {_product} {DownloadProductOption<TProductServerType, TStatusType>.FindVersion(args)}";
        }
    }
}