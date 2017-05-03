using System;
using cafe.Server.Jobs;
using cafe.Shared;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace cafe.Server.Controllers
{
    public class ProductController : Controller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ProductController).FullName);

        private readonly ProductJobRunner<ProductStatus> _jobRunner;
        private readonly string _productName;

        public ProductController(string productName, ProductJobRunner<ProductStatus> jobRunner)
        {
            Logger.Debug($"Initializing controller for {productName}");
            _productName = productName;
            _jobRunner = jobRunner;
        }

        protected JobRunStatus DoInstall(string version)
        {
            Logger.Info($"Scheduling {_productName} {version} to be installed");
            return _jobRunner.InstallJob.InstallOrUpgrade(version);
        }

        protected JobRunStatus DoDownload(string version)
        {
            Logger.Info($"Scheduling {_productName} {version} to be downloaded");
            return _jobRunner.DownloadJob.Download(version);
        }

        protected ProductStatus DoGetStatus()
        {
            Logger.Info($"Getting {_productName} status");
            var status = _jobRunner.ToStatus();
            Logger.Debug($"Status for {_productName} is {status}");
            return status;
        }

        protected T ExecuteFunctionWithErrorHandling<T>(Func<T> func, string description)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                Logger.Error($"Exception thrown while {description}: {e}");
                throw;
            }
        }

    }
}