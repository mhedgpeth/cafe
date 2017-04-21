using System;
using cafe.Shared;

namespace cafe.Chef
{
    public class ChefRunner : IChefRunner
    {
        private readonly Func<IChefProcess> _processCreator;

        public ChefRunner(Func<IChefProcess> processCreator)
        {
            _processCreator = processCreator;
        }

        public Result Run(IMessagePresenter presenter)
        {
            return Run(presenter, new RunChefPolicy());
        }

        public Result Run(IMessagePresenter presenter, IRunChefPolicy runChefPolicy)
        {
            presenter.ShowMessage($"Running chef {runChefPolicy}");
            runChefPolicy.PrepareEnvironmentForChefRun();
            var process = _processCreator();
            process.LogEntryReceived += (sender, entry) =>
            {
                presenter.ShowMessage(entry.Entry);
                entry.Log();
            };
            var argumentsForChefRun = runChefPolicy.ArgumentsForChefRun();
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