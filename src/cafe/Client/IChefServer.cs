using System.Threading.Tasks;
using cafe.Shared;
using RestEase;

namespace cafe.Client
{
    public interface IChefServer
    {
        [Get("status")]
        Task<ChefStatus> GetStatus();

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
        Task<ChefStatus> Pause();

        [Put("resume")]
        Task<ChefStatus> Resume();
    }
}