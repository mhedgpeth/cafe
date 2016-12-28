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
        Task<ScheduledTaskStatus> GetTaskStatus([Path]Guid id);

        [Get("recurring/{name}")]
        Task<RecurringTaskStatus> GetRecurringTaskStatus([Path]string name);

        [Put("recurring/{name}/pause")]
        Task<RecurringTaskStatus> PauseRecurringTask([Path]string name);

        [Put("recurring/{name}/resume")]
        Task<RecurringTaskStatus> ResumeRecurringTask([Path]string name);
    }
}