using System.Threading.Tasks;
using cafe.Shared;
using RestEase;

namespace cafe.Client
{
    public interface IChefServer
    {
        [Get("status")]
        Task<ChefStatus> GetChefStatus();
    }
}