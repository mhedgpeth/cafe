using System;

namespace cafe.CommandLine
{
    public abstract class Option
    {
        private readonly string _helpText;
        private readonly OptionSpecification _specification;

        protected Option(OptionSpecification specification, string helpText)
        {
            _specification = specification;
            _helpText = helpText;
        }

        public void Run(params string[] args)
        {
            if (_specification.HelpRequested(args))
            {
                ShowHelp();
            }
            else
            {
                RunCore();
            }
        }

        protected abstract void RunCore();

        public virtual void ShowHelp()
        {
            Console.Out.WriteLine($"Help: {_helpText}");
        }

        public bool IsSatisfiedBy(string[] args)
        {
            return _specification.IsSatisfiedBy(args);
        }
    }
}