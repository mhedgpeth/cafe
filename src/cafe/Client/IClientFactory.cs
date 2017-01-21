namespace cafe.Client
{
    public interface IClientFactory
    {
        IChefServer RestClientForChefServer();
    }
}