using System;
using System.Threading.Tasks;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Options.Chef
{
    public class DownloadProductOption<TProductServerType, TStatusType> : RunJobOption<TProductServerType>
        where TProductServerType : IProductServer<TStatusType>
        where TStatusType : ProductStatus
    {
        private readonly string _product;

        public DownloadProductOption(string product, Func<TProductServerType> productServerFactory, ISchedulerWaiter schedulerWaiter)
            : base(productServerFactory, schedulerWaiter,
                "downloads the provided version of chef")
        {
            _product = product;
        }

        protected override Task<JobRunStatus> RunJobCore(TProductServerType productServer, Argument[] args)
        {
            return productServer.Download(FindVersion(args));
        }

        public static string FindVersion(Argument[] args)
        {
            return args.FindValueFromLabel("version:").Value;
        }

        protected override string ToDescription(Argument[] args)
        {
            return $"Downloading {_product} {FindVersion(args)}";
        }
    }
}