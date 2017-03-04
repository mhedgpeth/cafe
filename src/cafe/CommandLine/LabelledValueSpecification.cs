namespace cafe.CommandLine
{
    public abstract class LabelledValueSpecification : OptionValueSpecification
    {
        private readonly string _label;

        protected LabelledValueSpecification(string label, string description, bool isRequired = true)
            : base(CreateDescription(label, description, isRequired), isRequired)
        {
            _label = label;
        }

        private static string CreateDescription(string label, string description, bool isRequired)
        {
            var start = !isRequired ? "[" : string.Empty;
            var end = !isRequired ? "]" : string.Empty;

            return $"{start}{label} ({description}){end}";
        }


        public override bool IsSatisfiedBy(int position, params Argument[] args)
        {
            return args.FindValueFromLabel(_label) != null;
        }

        public override Argument ParseArgument(string label, string value)
        {
            return string.IsNullOrEmpty(label) || string.Equals(label, _label) ? CreateValueArgument(value) : null;
        }

        protected abstract bool IsSatisfiedByValue(string value);

        private ValueArgument CreateValueArgument(string value)
        {
            return new ValueArgument(_label, value);
        }
    }
}