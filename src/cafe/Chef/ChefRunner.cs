using System;
using cafe.LocalSystem;
using cafe.Shared;

namespace cafe.Chef
{
    public class ChefRunner
    {
        private readonly Func<IChefProcess> _processCreator;
        private readonly IFileSystemCommands _fileSystemCommands;

        public ChefRunner(Func<IChefProcess> processCreator, IFileSystemCommands fileSystemCommands)
        {
            _processCreator = processCreator;
            _fileSystemCommands = fileSystemCommands;
        }

        public Result Run(IMessagePresenter presenter)
        {
            return Run(presenter, new NullBootstrapper());
        }


        public Result Run(IMessagePresenter presenter, IChefBootstrapper chefBootstrapper)
        {
            presenter.ShowMessage($"Running chef #{chefBootstrapper}");
            chefBootstrapper.PrepareEnvironmentForChefRun();
            var process = _processCreator();
            process.LogEntryReceived += (sender, entry) =>
            {
                presenter.ShowMessage(entry.Entry);
                entry.Log();
            };
            var result = process.Run(chefBootstrapper.ArgumentsForChefRun());
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