using cafe.Shared;
using NLog;
using RestEase;

namespace cafe.Client
{
    public class ClientFactory : IClientFactory
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ClientFactory).FullName);

        private readonly int _port;

        public ClientFactory(string hostname, int port)
        {
            Logger.Debug($"Creating clients on port {port}");
            Hostname = hostname;
            _port = port;
        }

        public string Hostname { get; set; }

        public IChefServer RestClientForChefServer()
        {
            return CreateRestClientFor<IChefServer>("chef");
        }

        public IJobServer RestClientForJobServer()
        {
            return CreateRestClientFor<IJobServer>("job");
        }


        private T CreateRestClientFor<T>(string serviceEndpoint)
        {
            var endpoint = $"http://{Hostname}:{_port}/api/{serviceEndpoint}";
            Logger.Debug($"Creating rest client for {typeof(T).FullName} at endpoint {endpoint}");
            return RestClient.For<T>(endpoint);
        }

        public IProductServer<ProductStatus> RestClientForInspecServer()
        {
            return CreateGenericProductRestClientFor("inspec");
        }

        private IProductServer<ProductStatus> CreateGenericProductRestClientFor(string serviceEndpoint)
        {
            return CreateRestClientFor<IProductServer<ProductStatus>>(serviceEndpoint);
        }

        public IProductServer<ProductStatus> GenericRestClientForChefServer()
        {
            return CreateGenericProductRestClientFor("chef");
        }

        public IProductServer<ProductStatus> RestClientForCafeProductServer()
        {
            return CreateGenericProductRestClientFor("cafe");
        }
    }
}
