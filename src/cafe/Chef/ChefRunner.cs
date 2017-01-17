using System;
using cafe.Shared;

namespace cafe.Chef
{
    public class ChefRunner
    {
        private readonly Func<IChefProcess> _processCreator;

        public ChefRunner(Func<IChefProcess> processCreator)
        {
            _processCreator = processCreator;
        }

        public Result Run(IMessagePresenter presenter)
        {
            return Run(presenter, new NullBootstrapper());
        }

        public Result Run(IMessagePresenter presenter, IChefBootstrapper chefBootstrapper)
        {
            presenter.ShowMessage($"Running chef {chefBootstrapper}");
            chefBootstrapper.PrepareEnvironmentForChefRun();
            var process = _processCreator();
            process.LogEntryReceived += (sender, entry) =>
            {
                presenter.ShowMessage(entry.Entry);
                entry.Log();
            };
            var argumentsForChefRun = chefBootstrapper.ArgumentsForChefRun();
            var result = process.Run(argumentsForChefRun);
            presenter.ShowMessage($"Finished running chef with result: {result}");
            return result;
        }

        public static RunListChefBootstrapSettings ParseRunList(string runList)
        {
            return new RunListChefBootstrapSettings()
            {
                RunList = runList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            };
        }
    }
}