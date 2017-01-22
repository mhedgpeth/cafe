using System;
using cafe.CommandLine;
using cafe.Shared;
using NLog;

namespace cafe.Options
{
    public abstract class ServerConnectionOption<T> : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger("cafe.Options.ServerConnectionOption");

        private readonly Func<T> _serverFactory;

        protected ServerConnectionOption(Func<T> serverFactory, string helpText) : base(helpText)
        {
            _serverFactory = serverFactory;
        }

        protected sealed override Result RunCore(string[] args)
        {
            try
            {
                var schedulerServer = _serverFactory();
                return RunCore(schedulerServer, args);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, $"An exception occurred while {ToDescription(args)}");
                Presenter.ShowMessage("The server is not currently running", Logger);
                return Result.Failure("Could not establish connection with cafe server");
            }
        }

        protected abstract Result RunCore(T client, string[] args);
    }
}