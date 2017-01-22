using System.Threading.Tasks;
using cafe.Shared;
using RestEase;

namespace cafe.Client
{
    public interface IProductServer<T> where T : ProductStatus
    {
        [Get("status")]
        Task<T> GetStatus();

        [Put("download")]
        Task<JobRunStatus> Download(string version);

        [Put("install")]
        Task<JobRunStatus> Install(string version);
    }

    public interface IChefServer : IProductServer<ChefStatus>
    {
        [Put("run")]
        Task<JobRunStatus> RunChef();

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