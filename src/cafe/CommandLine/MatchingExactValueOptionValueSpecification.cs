namespace cafe.CommandLine
{
    public class MatchingExactValueOptionValueSpecification : OptionValueSpecification
    {
        private readonly string _value;

        public MatchingExactValueOptionValueSpecification(string value)
            : base(value)
        {
            _value = value;
        }

        public override bool IsSatisfiedBy(string value)
        {
            return string.Equals(value, _value);
        }
    }
}