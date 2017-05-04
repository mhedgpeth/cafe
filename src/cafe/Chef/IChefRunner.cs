using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Chef
{
    public interface IChefRunner
    {
        Result Run(IMessagePresenter presenter);
        Result Run(IMessagePresenter presenter, IRunChefPolicy chefBootstrapper);
    }
}