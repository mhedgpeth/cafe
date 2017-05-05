using System;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Chef
{
    public class ChefRunner : IChefRunner
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefRunner).FullName);

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
                Logger.Debug($"Received log message from chef run: {entry.Entry}");
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