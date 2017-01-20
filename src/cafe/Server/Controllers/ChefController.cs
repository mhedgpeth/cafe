using cafe.Chef;
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

        private readonly ChefJobRunner _chefJobRunner = StructureMapResolver.Container.GetInstance<ChefJobRunner>();

        [HttpPut("run")]
        public ScheduledTaskStatus RunChef()
        {
            return _chefJobRunner.RunChefJob.Run();
        }

        [HttpPut("install")]
        [HttpPut("upgrade")]
        public ScheduledTaskStatus InstallChef(string version)
        {
            Logger.Info($"Scheduling chef {version} to be installed");
            return _chefJobRunner.InstallChefJob.InstallOrUpgrade(version);
        }

        [HttpPut("download")]
        public ScheduledTaskStatus DownloadChef(string version)
        {
            Logger.Info($"Scheduling chef {version} to be downloaded");
            return _chefJobRunner.DownloadChefJob.Download(version);
        }

        [HttpGet("status")]
        public ChefStatus GetChefStatus()
        {
            Logger.Info("Getting chef status");
            var product = StructureMapResolver.Container.GetInstance<ChefProduct>();
            return product.ToChefStatus();
        }

        [HttpPut("bootstrap/policy")]
        public ScheduledTaskStatus BootstrapChef(string config, string validator, string policyName, string policyGroup)
        {
            Logger.Info($"Bootstrapping chef with policy {policyName} and group: {policyGroup}");
            return _chefJobRunner.RunChefJob.Bootstrap(CreateChefBootstrapper(config, validator,
                    new PolicyChefBootstrapSettings {PolicyGroup = policyGroup, PolicyName = policyName}));
        }

        [HttpPut("bootstrap/runList")]
        public ScheduledTaskStatus BootstrapChef(string config, string validator, string runList)
        {
            var description = $"Bootstrapping chef with run list {runList}";
            Logger.Info(description);
            return _chefJobRunner.RunChefJob.Bootstrap(CreateChefBootstrapper(config, validator, ChefRunner.ParseRunList(runList)));
        }

        private static ChefBootstrapper CreateChefBootstrapper(string config, string validator,
            BootstrapSettings bootstrapSettings)
        {
            return new ChefBootstrapper(StructureMapResolver.Container.GetInstance<IFileSystemCommands>(), config,
                validator, bootstrapSettings);
        }

        [HttpGet("status")]
        public RecurringTaskStatus GetRunChefStatus(string name)
        {
            Logger.Info($"Getting recurring task named {name}");
            var status = _chefJobRunner.RunChefJob.ToStatus();
            Logger.Debug($"Status for recurring task {name} is {status}");
            return status;
        }

        [HttpPut("pause")]
        public RecurringTaskStatus PauseChef(string name)
        {
            Logger.Info($"Pausing recurring task {name}");
            var status = _chefJobRunner.RunChefJob.Pause();
            Logger.Debug($"Finished pausing task {name} with new status of {status}");
            return status;
        }

        [HttpPut("recurring/{name}/resume")]
        public RecurringTaskStatus ResumeRecurringTask(string name)
        {
            Logger.Info($"Resuming recurring task {name}");
            var status = _chefJobRunner.RunChefJob.Resume();
            Logger.Debug($"Finished resuming task {name} with new status of {status}");
            return status;
        }

    }
}