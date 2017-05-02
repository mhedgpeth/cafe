using System;
using cafe.Chef;
using cafe.LocalSystem;
using cafe.Server.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace cafe.Server.Controllers
{
    [Route("api/[controller]")]
    public class CafeController : ProductController
    {
        public CafeController() : base(CafeJobRunner)
        {
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
            throw new InvalidOperationException("Runtime identifier not found in cafe.deps.json");
        }
    }
}