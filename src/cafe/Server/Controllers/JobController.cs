using System;
using cafe.Server.Jobs;
using cafe.Shared;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace cafe.Server.Controllers
{
    [Route("api/[controller]")]
    public class JobController : Controller
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(JobController).FullName);

        private readonly JobRunner _jobRunner = StructureMapResolver.Container.GetInstance<JobRunner>();

        [HttpGet("{id}")]
        public JobRunStatus GetJobRunStatus(Guid id)
        {
            Logger.Info($"Getting status of task with id {id}");
            var status = _jobRunner.FindStatusById(id);
            Logger.Debug($"Status for task {id} is {status}");
            return status;
        }

        [HttpGet("status")]
        public JobRunnerStatus GetStatus()
        {
            Logger.Info($"Getting job runner status");
            var status = _jobRunner.ToStatus();
            Logger.Debug($"Status for job runner is {status}");
            return status;
        }
    }
}