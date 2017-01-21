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
        Task<ServerStatus> GetStatus();

        [Put("run")]
        Task<JobRunStatus> RunChef();

        [Put("download")]
        Task<JobRunStatus> DownloadChef(string version);

        [Put("install")]
        Task<JobRunStatus> InstallChef(string version);

        [Put("bootstrap/policy")]
        Task<JobRunStatus> BootstrapChef(string config, string validator, string policyName, string policyGroup);

        [Put("bootstrap/runList")]
        Task<JobRunStatus> BootstrapChef(string config, string validator, string runList);

        [Put("pause")]
        Task<ServerStatus> Pause();

        [Put("resume")]
        Task<ServerStatus> Resume();

        [Get("task/{id}")]
        Task<JobRunStatus> GetTaskStatus([Path]Guid id);
    }
}