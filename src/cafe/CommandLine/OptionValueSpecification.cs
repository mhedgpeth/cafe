using System.Text.RegularExpressions;

namespace cafe.CommandLine
{
    public abstract class OptionValueSpecification
    {
        private readonly string _description;

        protected OptionValueSpecification(string description)
        {
            _description = description;
        }

        public abstract bool IsSatisfiedBy(string value);

        public static OptionValueSpecification ForExactValue(string value)
        {
            return new MatchingExactValueOptionValueSpecification(value);
        }

        public static OptionValueSpecification ForVersion()
        {
            return new MatchingRegexOptionValueSpecification("[version]", new Regex(@"\d+\.\d+\.\d+"));
        }

        public override string ToString()
        {
            return _description;
        }

        public static OptionValueSpecification ForAnyValue(string description)
        {
            return new MatchingRegexOptionValueSpecification($"[{description}]", new Regex(".*"));
        }
    }
}