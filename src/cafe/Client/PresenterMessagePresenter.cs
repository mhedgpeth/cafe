using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Client
{
    public class PresenterMessagePresenter : IMessagePresenter
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(PresenterMessagePresenter).FullName);

        public void ShowMessage(string message)
        {
            Presenter.ShowMessage(message, Logger);
        }
    }
}