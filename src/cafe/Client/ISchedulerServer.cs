using System;
using System.Threading.Tasks;
using cafe.Shared;
using RestEase;

namespace cafe.Client
{
    public interface ISchedulerServer
    {
        [Get("status")]
        Task<SchedulerStatus> GetStatus();

        [Get("task/{id}")]
        Task<ScheduledTaskStatus> GetTaskStatus(Guid id);
    }
}