using System;
using System.Threading.Tasks;
using cafe.Shared;
using Microsoft.AspNetCore.Mvc;
using RestEase;

namespace cafe.Client
{
    public interface IChefServer
    {
        [Get("status")]
        Task<SchedulerStatus> GetStatus();

        [Put("run")]
        Task<ScheduledTaskStatus> RunChef();

        [Put("download")]
        Task<ScheduledTaskStatus> DownloadChef(string version);

        [Put("install")]
        Task<ScheduledTaskStatus> InstallChef(string version);

        [Put("bootstrap/policy")]
        Task<ScheduledTaskStatus> BootstrapChef(string config, string validator, string policyName, string policyGroup);

        [Put("bootstrap/runList")]
        Task<ScheduledTaskStatus> BootstrapChef(string config, string validator, string runList);

        [Put("pause")]
        Task<SchedulerStatus> Pause();

        [Put("resume")]
        Task<SchedulerStatus> Resume();

        [Get("task/{id}")]
        Task<ScheduledTaskStatus> GetTaskStatus([Path]Guid id);
    }
}