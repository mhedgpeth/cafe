namespace cafe.CommandLine
{
    public class ValueArgument : Argument
    {
        private readonly string _label;
        private readonly string _value;

        public ValueArgument(string label, string value)
        {
            _label = label;
            _value = value;
        }

        public string Label => _label;

        public string Value => _value;

        public override string ToString()
        {
            return $"{_label} {_value}";
        }
    }
}