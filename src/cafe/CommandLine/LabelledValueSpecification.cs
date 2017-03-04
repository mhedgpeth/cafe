using System;

namespace cafe.CommandLine
{
    public abstract class LabelledValueSpecification : OptionValueSpecification
    {
        private readonly string _label;

        protected LabelledValueSpecification(string label, string description, bool isRequired = true) : base(description, isRequired)
        {
            _label = label;
        }

        public override bool IsSatisfiedBy(int position, params Argument[] args)
        {
            return args.FindValueFromLabel(_label) != null;
        }

        public override Argument ParseArgument(string label, string value)
        {
            return CreateValueArgument(value);
        }

        protected abstract bool IsSatisfiedByValue(string value);

        private ValueArgument CreateValueArgument(string value)
        {
            return new ValueArgument(_label, value);
        }
    }
}