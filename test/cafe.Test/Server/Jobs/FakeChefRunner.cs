using cafe.Chef;
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

        public Result Run(IMessagePresenter presenter, IChefBootstrapper chefBootstrapper)
        {
            WasRun = true;
            Bootstrapper = chefBootstrapper;
            return Result.Successful();
        }

        public IChefBootstrapper Bootstrapper { get; private set; }

        public bool WasRun { get; private set; }
    }
}