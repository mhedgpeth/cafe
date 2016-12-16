using System.Text.RegularExpressions;

namespace cafe.CommandLine
{
    public class OptionValueSpecification
    {
        private readonly string _description;
        private readonly Regex _expression;

        private OptionValueSpecification(string description, Regex expression)
        {
            _description = description;
            _expression = expression;
        }

        public bool IsSatisfiedBy(string argument)
        {
            return _expression.IsMatch(argument);
        }

        public static OptionValueSpecification ForExactValue(string value)
        {
            return new OptionValueSpecification(value, new Regex(value));
        }

        public static OptionValueSpecification ForVersion()
        {
            return new OptionValueSpecification("[version]", new Regex(@"\d+\.\d+\.\d+"));
        }

        public override string ToString()
        {
            return _description;
        }
    }
}