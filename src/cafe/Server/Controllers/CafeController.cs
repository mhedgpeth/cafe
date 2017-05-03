using cafe.Chef;
using cafe.LocalSystem;
using cafe.Server.Jobs;
using cafe.Shared;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace cafe.Server.Controllers
{
    [Route("api/[controller]")]
    public class CafeController : ProductController
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(CafeController).FullName);

        public CafeController() : base("cafe", CafeJobRunner)
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

        private static readonly GenericProductJobRunner CafeJobRunner = CreateJobRunner();

        private static GenericProductJobRunner CreateJobRunner()
        {
            const string product = "cafe";
            var commands = new FileSystemCommandsBoundary();
            var fileSystem = new FileSystem(new EnvironmentBoundary(), commands);
            var resolver = new CafeDownloadUrlResolver(FindRuntimeIdentifier(commands));
            var cafeInstaller = new CafeInstaller(commands, resolver, new ProcessExecutor(() => new ProcessBoundary()));
            return new GenericProductJobRunner(StructureMapResolver.Container.GetInstance<JobRunner>(),
                InspecController.CreateDownloadJob(fileSystem, product, resolver),
                InspecController.CreateInstallJob(cafeInstaller));
        }

        private static string  FindRuntimeIdentifier(IFileSystemCommands commands)
        {
            var deps = commands.ReadAllText("cafe.deps.json");
            var identifiers = new[]{"win10", "win8", "win7"};
            foreach (var identifier in identifiers)
            {
                if(deps.Contains(identifier))
                {
                    return identifier;
                }
            }
            Logger.Warn(
                "Runtime identifier not found in cafe.deps.json, so we are assuming this is running on windows 10");
            return "win10";
        }
    }
}