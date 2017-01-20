using System;
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
        public SchedulerStatus GetStatus()
        {
            Logger.Info($"Getting chef status");
            var status = _chefJobRunner.ToStatus();
            Logger.Debug($"Status for chef is {status}");
            return status;
        }

        [HttpPut("pause")]
        public SchedulerStatus Pause()
        {
            Logger.Info($"Pausing chef");
            _chefJobRunner.RunChefJob.Pause();
            var status = _chefJobRunner.ToStatus();
            Logger.Debug($"Finished pausing chef with new status of {status}");
            return status;
        }

        [HttpPut("resume")]
        public SchedulerStatus Resume()
        {
            Logger.Info($"Resuming chef");
            _chefJobRunner.RunChefJob.Resume();
            var status = _chefJobRunner.ToStatus();
            Logger.Debug($"Finished resuming chef with new status of {status}");
            return status;
        }

        [HttpGet("task/{id}")]
        public ScheduledTaskStatus GetTaskStatus(Guid id)
        {
            Logger.Info($"Getting status of task with id {id}");
            var status = _chefJobRunner.FindStatusById(id);
            Logger.Debug($"Status for task {id} is {status}");
            return status;
        }
    }
}