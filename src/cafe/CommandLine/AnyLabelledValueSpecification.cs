namespace cafe.CommandLine
{
    public class AnyLabelledValueSpecification : LabelledValueSpecification
    {
        public AnyLabelledValueSpecification(string label, string description) : base(label, description)
        {
        }

        protected override bool IsSatisfiedByValue(string value)
        {
            return true;
        }
    }
}