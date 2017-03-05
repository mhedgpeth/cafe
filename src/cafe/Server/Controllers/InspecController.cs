using System;
using cafe.Chef;
using cafe.LocalSystem;
using cafe.Server.Jobs;
using cafe.Shared;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NodaTime;

namespace cafe.Server.Controllers
{
    [Route("api/[controller]")]
    public class InspecController : Controller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(InspecController).FullName);

        private static readonly InspecJobRunner InspecJobRunner = CreateJobRunner();

        public static InspecJobRunner CreateJobRunner()
        {
            var commands = new FileSystemCommandsBoundary();
            var fileSystem = new FileSystem(new EnvironmentBoundary(), commands);
            const string prefix = "inspec";
            var product = prefix;
            return new InspecJobRunner(StructureMapResolver.Container.GetInstance<JobRunner>(),
                CreateDownloadJob(fileSystem, product, prefix, "2012"),
                CreateInstallJob(product, fileSystem, commands, prefix, InstalledProductsFinder.IsInspec));
        }

        public static InstallJob CreateInstallJob(string product, FileSystem fileSystem, FileSystemCommandsBoundary commands, string prefix, Func<ProductInstallationMetaData, bool> productMatcher)
        {
            var chefProduct = new ChefProduct(product, new InstalledProductsFinder(),
                new ProductInstaller(fileSystem, new ProcessExecutor(() => new ProcessBoundary()), commands, prefix, $@"{ServerSettings.Instance.InstallRoot}\opscode"),
                productMatcher);
            return new InstallJob(chefProduct, SystemClock.Instance);
        }

        public static DownloadJob CreateDownloadJob(FileSystem fileSystem, string product, string prefix, string target)
        {
            return new DownloadJob(new Downloader(new FileDownloader(), fileSystem, product, prefix, target),
                SystemClock.Instance);
        }

        [HttpPut("install")]
        [HttpPut("upgrade")]
        public JobRunStatus InstallChef(string version)
        {
            Logger.Info($"Scheduling chef {version} to be installed");
            return InspecJobRunner.InstallJob.InstallOrUpgrade(version);
        }

        [HttpPut("download")]
        public JobRunStatus DownloadChef(string version)
        {
            Logger.Info($"Scheduling chef {version} to be downloaded");
            return InspecJobRunner.DownloadJob.Download(version);
        }

        [HttpGet("status")]
        public ProductStatus GetStatus()
        {
            Logger.Info($"Getting chef status");
            var status = InspecJobRunner.ToStatus();
            Logger.Debug($"Status for chef is {status}");
            return status;
        }
    }
}