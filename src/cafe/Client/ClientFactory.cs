using cafe.Shared;
using RestEase;

namespace cafe.Client
{
    public class ClientFactory
    {
        public IChefServer RestClientForChefServer()
        {
            return CreateRestClientFor<IChefServer>("chef");
        }

        private static T CreateRestClientFor<T>(string serviceEndpoint)
        {
            return RestClient.For<T>($"http://localhost:{ServerSettings.Port}/api/{serviceEndpoint}");
        }

        public ISchedulerServer RestClientForSchedulerServer()
        {
            return CreateRestClientFor<ISchedulerServer>("scheduler");
        }
    }
}