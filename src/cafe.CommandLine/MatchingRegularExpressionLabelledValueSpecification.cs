using System.Text.RegularExpressions;

namespace cafe.CommandLine
{
    public class MatchingRegularExpressionLabelledValueSpecification : LabelledValueSpecification
    {
        private readonly Regex _regularExpression;

        public MatchingRegularExpressionLabelledValueSpecification(string label, Regex regularExpression, string description) : base(label, description)
        {
            _regularExpression = regularExpression;
        }

        protected override bool IsSatisfiedByValue(string value)
        {
            return _regularExpression.IsMatch(value);
        }
    }
}