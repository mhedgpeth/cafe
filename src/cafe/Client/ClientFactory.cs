using cafe.Options;
using NLog;
using RestEase;

namespace cafe.Client
{
    public class ClientFactory : IClientFactory
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ClientFactory).FullName);

        private readonly int _port;

        public ClientFactory(int port)
        {
            Logger.Debug($"Creating clients on port {port}");
            _port = port;
        }

        public IChefServer RestClientForChefServer()
        {
            return CreateRestClientFor<IChefServer>("chef");
        }

        private T CreateRestClientFor<T>(string serviceEndpoint)
        {
            var endpoint = $"http://localhost:{_port}/api/{serviceEndpoint}";
            Logger.Debug($"Creating rest client for {typeof(T).FullName} at endpoint {endpoint}");
            return RestClient.For<T>(endpoint);
        }

        public ISchedulerServer RestClientForSchedulerServer()
        {
            return CreateRestClientFor<ISchedulerServer>("scheduler");
        }
    }
}