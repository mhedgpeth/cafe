using cafe.Chef;
using cafe.CommandLine.LocalSystem;
using cafe.LocalSystem;
using cafe.Server.Jobs;
using cafe.Shared;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace cafe.Server.Controllers
{
    [Route("api/[controller]")]
    public class ChefController : Controller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefController).FullName);

        private static readonly ChefJobRunner ChefJobRunner = CreateChefJobRunner();

        private static ChefJobRunner CreateChefJobRunner()
        {
            var commands = new FileSystemCommandsBoundary();
            var fileSystem = new FileSystem(new EnvironmentBoundary(), commands);
            const string prefix = "chef-client";
            var product = "chef";

            var chefDownloadUrlResolver = new ChefDownloadUrlResolver(product, prefix, "2012r2");
            return new ChefJobRunner(StructureMapResolver.Container.GetInstance<JobRunner>(),
                InspecController.CreateDownloadJob(fileSystem, product, chefDownloadUrlResolver),
                InspecController.CreateInstallJob(product, fileSystem, commands, InstalledProductsFinder.IsChefClient,
                    chefDownloadUrlResolver),
                StructureMapResolver.Container.GetInstance<RunChefJob>());
        }


        [HttpPut("run")]
        public JobRunStatus RunChef()
        {
            return ChefJobRunner.RunChefJob.Run();
        }

        [HttpPut("install")]
        [HttpPut("upgrade")]
        public JobRunStatus InstallChef(string version)
        {
            Logger.Info($"Scheduling chef {version} to be installed");
            return ChefJobRunner.InstallJob.InstallOrUpgrade(version);
        }

        [HttpPut("download")]
        public JobRunStatus DownloadChef(string version)
        {
            Logger.Info($"Scheduling chef {version} to be downloaded");
            return ChefJobRunner.DownloadJob.Download(version);
        }

        [HttpPut("bootstrap/policy")]
        public JobRunStatus BootstrapChef(string config, string validator, string policyName, string policyGroup)
        {
            Logger.Info($"Bootstrapping chef with policy {policyName} and group: {policyGroup}");
            return ChefJobRunner.RunChefJob.Bootstrap(CreateChefBootstrapper(config, validator,
                new PolicyChefBootstrapSettings {PolicyGroup = policyGroup, PolicyName = policyName}));
        }

        [HttpPut("bootstrap/runList")]
        public JobRunStatus BootstrapChef(string config, string validator, string runList)
        {
            var description = $"Bootstrapping chef with run list {runList}";
            Logger.Info(description);
            return ChefJobRunner.RunChefJob.Bootstrap(
                CreateChefBootstrapper(config, validator, ChefRunner.ParseRunList(runList)));
        }

        private static IRunChefPolicy CreateChefBootstrapper(string config, string validator,
            BootstrapSettings bootstrapSettings)
        {
            return new BootstrapChefPolicy(StructureMapResolver.Container.GetInstance<IFileSystemCommands>(), config,
                validator, bootstrapSettings);
        }

        [HttpPut("pause")]
        public ChefStatus Pause()
        {
            Logger.Info($"Pausing chef");
            ChefJobRunner.RunChefJob.Pause();
            var status = ChefJobRunner.ToStatus();
            Logger.Debug($"Finished pausing chef with new status of {status}");
            return status;
        }

        [HttpPut("resume")]
        public ChefStatus Resume()
        {
            Logger.Info($"Resuming chef");
            ChefJobRunner.RunChefJob.Resume();
            var status = ChefJobRunner.ToStatus();
            Logger.Debug($"Finished resuming chef with new status of {status}");
            return status;
        }

        [HttpGet("status")]
        public ChefStatus GetStatus()
        {
            Logger.Info($"Getting chef status");
            var status = ChefJobRunner.ToStatus();
            Logger.Debug($"Status for chef is {status}");
            return status;
        }
    }
}