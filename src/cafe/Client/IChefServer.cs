using System.Threading.Tasks;
using cafe.Shared;
using RestEase;

namespace cafe.Client
{
    public interface IChefServer
    {
        [Get("status")]
        Task<ChefStatus> GetChefStatus();

        [Put("run")]
        Task<ScheduledTaskStatus> RunChef();

        [Put("download")]
        Task<ScheduledTaskStatus> DownloadChef(string version);

        [Put("install")]
        Task<ScheduledTaskStatus> InstallChef(string version);
    }
}