using RestEase;

namespace cafe.Client
{
    public class ClientFactory
    {
        private readonly int _port;

        public ClientFactory(int port)
        {
            _port = port;
        }

        public IChefServer RestClientForChefServer()
        {
            return CreateRestClientFor<IChefServer>("chef");
        }

        private T CreateRestClientFor<T>(string serviceEndpoint)
        {
            return RestClient.For<T>($"http://localhost:{_port}api/{serviceEndpoint}");
        }

        public ISchedulerServer RestClientForSchedulerServer()
        {
            return CreateRestClientFor<ISchedulerServer>("scheduler");
        }
    }
}