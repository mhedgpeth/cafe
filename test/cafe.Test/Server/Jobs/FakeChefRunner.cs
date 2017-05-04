using cafe.Chef;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Test.Server.Jobs
{
    public class FakeChefRunner : IChefRunner
    {
        public Result Run(IMessagePresenter presenter)
        {
            WasRun = true;
            return Result.Successful();
        }

        public Result Run(IMessagePresenter presenter, IRunChefPolicy chefBootstrapper)
        {
            WasRun = true;
            Bootstrapper = chefBootstrapper;
            return Result.Successful();
        }

        public IRunChefPolicy Bootstrapper { get; private set; }

        public bool WasRun { get; private set; }
    }
}