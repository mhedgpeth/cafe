using System;
using System.Threading.Tasks;
using cafe.Shared;
using RestEase;

namespace cafe.Client
{
    public interface IJobServer
    {
        [Get("status")]
        Task<JobRunnerStatus> GetStatus();

        [Get("{id}")]
        Task<JobRunStatus> GetJobRunStatus([Path]Guid id, int previousIndex);
    }
}