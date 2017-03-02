using System.Text.RegularExpressions;

namespace cafe.CommandLine
{
    public abstract class OptionValueSpecification
    {
        private readonly string _description;
        private readonly bool _isRequired;

        protected OptionValueSpecification(string description, bool isRequired = true)
        {
            _description = description;
            _isRequired = isRequired;
        }

        public abstract bool IsSatisfiedBy(int position, params Argument[] args);

        public static OptionValueSpecification ForVersion()
        {
            return new MatchingRegularExpressionLabelledValueSpecification("version:", new Regex(@"\d+\.\d+\.\d+"), "the version");
        }

        public override string ToString()
        {
            return _description;
        }

        public abstract Argument ParseArgument(string label, string value);

        public static OptionValueSpecification ForCommand(string command)
        {
            return new CommandOptionValueSpecification(command, command);
        }

        public static OptionValueSpecification ForValue(string label, string description)
        {
            return new AnyLabelledValueSpecification(label, description);
        }

        public static OptionValueSpecification ForOptionalCommand(string command)
        {
            return new CommandOptionValueSpecification(command, command, false);
        }

        public static OptionValueSpecification OptionalHelpCommand()
        {
            const string helpCommand = "-h";
            return new CommandOptionValueSpecification(helpCommand, helpCommand, false);
        }

        public bool IsRequired => _isRequired;
    }
}