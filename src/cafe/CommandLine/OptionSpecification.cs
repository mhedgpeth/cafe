using System;
using System.Linq;

namespace cafe.CommandLine
{
    public class OptionSpecification
    {
        private readonly string[] _pattern;

        public OptionSpecification(params string[] pattern)
        {
            _pattern = pattern;
        }

        public bool IsSatisfiedBy(params string[] args)
        {
            var trimmedArguments = TrimLastArgumentIfItIsHelpRequest(args);
            if (_pattern.Length == trimmedArguments.Length)
            {
                for (int i = 0; i < _pattern.Length; i++)
                {
                    if (_pattern[i] != trimmedArguments[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private string[] TrimLastArgumentIfItIsHelpRequest(string[] args)
        {
            if (!HelpRequested(args)) return args;
            var list = args.ToList();
            list.Remove("-h");
            return list.ToArray();
        }

        public override string ToString()
        {
            return string.Join(" ", _pattern);
        }

        public bool HelpRequested(string[] args)
        {
            return args.LastOrDefault() == "-h";
        }
    }
}