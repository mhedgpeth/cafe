namespace cafe.CommandLine
{
    public class AnyLabelledValueSpecification : LabelledValueSpecification
    {
        public AnyLabelledValueSpecification(string label, string description, bool isRequired = true)
            : base(label, description, isRequired)
        {
        }

        protected override bool IsSatisfiedByValue(string value)
        {
            return true;
        }
    }
}