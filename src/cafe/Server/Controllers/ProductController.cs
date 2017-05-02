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

        public ProductController(ProductJobRunner<ProductStatus> jobRunner)
        {
            _jobRunner = jobRunner;
        }

        [HttpPut("install")]
        [HttpPut("upgrade")]
        public JobRunStatus InstallChef(string version)
        {
            Logger.Info($"Scheduling chef {version} to be installed");
            return _jobRunner.InstallJob.InstallOrUpgrade(version);
        }

        [HttpPut("download")]
        public JobRunStatus DownloadChef(string version)
        {
            Logger.Info($"Scheduling chef {version} to be downloaded");
            return _jobRunner.DownloadJob.Download(version);
        }

        [HttpGet("status")]
        public ProductStatus GetStatus()
        {
            Logger.Info("Getting chef status");
            var status = _jobRunner.ToStatus();
            Logger.Debug($"Status for chef is {status}");
            return status;
        }
    }
}