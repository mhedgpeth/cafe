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
            presenter.ShowMessage("Running chef");
            var process = _processCreator();
            process.LogEntryReceived += (sender, entry) =>
            {
                presenter.ShowMessage(entry.Entry);
                entry.Log();
            };
            var result = process.Run();
            presenter.ShowMessage($"Finished running chef with result: {result}");
            return result;
        }
    }
}