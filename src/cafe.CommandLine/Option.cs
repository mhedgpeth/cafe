using System;
using System.Linq;
using System.Net.Http;
using NLog;

namespace cafe.CommandLine
{
    public abstract class Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(Option).FullName);

        private readonly string _helpText;
        private readonly bool _showHelpContext;

        protected Option(string helpText, bool showHelpContext = true)
        {
            _helpText = helpText;
            _showHelpContext = showHelpContext;
        }

        public Result Run(params Argument[] args)
        {
            Result result = null;
            var description = ToDescription(args);
            try
            {
                if (_showHelpContext)
                {
                    Presenter.NewLine();
                    Presenter.ShowMessage($"{description}:", Logger);
                    Presenter.NewLine();

                }
                result = RunCore(args);
            }
            catch (AggregateException ae)
            {
                var inner = ae.InnerExceptions.FirstOrDefault(e => e is HttpRequestException);
                if (inner != null)
                {
                    result = BadConnectionFailureFromException(ae);
                }
                else
                {
                    result = GenericFailureFromException(ae);
                }
            }
            catch (HttpRequestException re)
            {
                result = BadConnectionFailureFromException(re);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An unexpected error occurred while executing this option");
                result = GenericFailureFromException(ex);
            }
            finally
            {
                if (_showHelpContext)
                {
                    Presenter.NewLine();
                    Presenter.ShowMessage($"Finished {description} with result: {result}", Logger);
                }
            }
            return result;
        }

        private Result BadConnectionFailureFromException(Exception ex)
        {
            Logger.Info(ex, "Could not connect to the server and thus got exception");

            return Result.Failure("A connection to the server could not be made. Make sure it's running.");
        }

        private static Result GenericFailureFromException(Exception ex)
        {
            return Result.Failure($"An unexpected error occurred while executing this option: {ex.Message}");
        }

        protected abstract string ToDescription(Argument[] args);

        protected abstract Result RunCore(Argument[] args);

        public override string ToString()
        {
            return _helpText;
        }

        public virtual void NotifyHelpWasShown() { }
    }
}