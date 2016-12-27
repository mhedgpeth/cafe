using System;
using cafe.Client;
using cafe.Shared;
using NLog;

namespace cafe.CommandLine
{
    public abstract class Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(SchedulerWaiter).FullName);

        private readonly string _helpText;
        private readonly OptionSpecification _specification;

        protected Option(OptionSpecification specification, string helpText)
        {
            _specification = specification;
            _helpText = helpText;
        }

        public Result Run(params string[] args)
        {
            if (_specification.HelpRequested(args))
            {
                ShowHelp();
                return Result.Successful();
            }
            try
            {
                return RunCore(args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An unexpected error occurred while executing this option");
                return Result.Failure($"An unexpected error occurred while executing this option: {ex.Message}");
            }
        }

        protected abstract Result RunCore(string[] args);

        public virtual void ShowHelp()
        {
            Console.Out.WriteLine($"Help: {_helpText}");
        }

        public bool IsSatisfiedBy(string[] args)
        {
            return _specification.IsSatisfiedBy(args);
        }

        public override string ToString()
        {
            return $"{_specification} ({_helpText})";
        }
    }
}