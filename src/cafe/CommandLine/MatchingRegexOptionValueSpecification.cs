using System.Text.RegularExpressions;

namespace cafe.CommandLine
{
    public class MatchingRegexOptionValueSpecification : OptionValueSpecification
    {
        private readonly Regex _expression;

        public MatchingRegexOptionValueSpecification(string description, Regex expression) : base(description)
        {
            _expression = expression;
        }

        public override bool IsSatisfiedBy(string value)
        {
            return _expression.IsMatch(value);
        }

    }
}