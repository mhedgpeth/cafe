using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Chef
{
    public interface IDownloader
    {
        string Product { get; }
        Result Download(string version, IMessagePresenter messagePresenter);
    }
}