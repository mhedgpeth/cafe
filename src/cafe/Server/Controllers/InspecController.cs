using System;
using cafe.Chef;
using cafe.CommandLine.LocalSystem;
using cafe.LocalSystem;
using cafe.Server.Jobs;
using cafe.Shared;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace cafe.Server.Controllers
{
    [Route("api/[controller]")]
    public class InspecController : ProductController
    {
        public InspecController() : base("inspec", InspecJobRunner)
        {
        }

        [HttpPut("install")]
        [HttpPut("upgrade")]
        public JobRunStatus InstallProduct(string version)
        {
            return ExecuteFunctionWithErrorHandling(() => DoInstall(version), $"installing {version}");
        }

        [HttpPut("download")]
        public JobRunStatus DownloadProduct(string version)
        {
            return ExecuteFunctionWithErrorHandling(() => DoDownload(version), $"downloading {version}");
        }

        [HttpGet("status")]
        public ProductStatus GetStatus()
        {
            return ExecuteFunctionWithErrorHandling(DoGetStatus, "getting status");
        }

        private static readonly GenericProductJobRunner InspecJobRunner = CreateJobRunner();

        private static GenericProductJobRunner CreateJobRunner()
        {
            const string prefix = "inspec";
            var downloadUrlResolver = new ChefDownloadUrlResolver(prefix, prefix, "2012");
            var productName = prefix;

            var commands = new FileSystemCommandsBoundary();
            var fileSystem = new FileSystem(new EnvironmentBoundary(), commands);
            return new GenericProductJobRunner(StructureMapResolver.Container.GetInstance<JobRunner>(),
                CreateDownloadJob(fileSystem, productName, downloadUrlResolver),
                CreateInstallJob(productName, fileSystem, commands, InstalledProductsFinder.IsInspec,
                    downloadUrlResolver));
        }

        public static InstallJob CreateInstallJob(string product, FileSystem fileSystem,
            FileSystemCommandsBoundary commands, Func<ProductInstallationMetaData, bool> productMatcher,
            IDownloadUrlResolver downloadUrlResolver)
        {
            var chefProduct = new ChefProduct(product, new InstalledProductsFinder(),
                new ProductInstaller(fileSystem, new ProcessExecutor(() => new ProcessBoundary()), commands,
                    $@"{ServerSettings.Instance.InstallRoot}\opscode", downloadUrlResolver),
                productMatcher);
            return CreateInstallJob(chefProduct);
        }

        public static InstallJob CreateInstallJob(IInstaller chefProduct)
        {
            return new InstallJob(chefProduct, SystemClock.Instance);
        }

        public static DownloadJob CreateDownloadJob(FileSystem fileSystem, string product,
            IDownloadUrlResolver downloadUrlResolver)
        {
            return new DownloadJob(new Downloader(new FileDownloader(), fileSystem, product, downloadUrlResolver),
                SystemClock.Instance);
        }
    }
}