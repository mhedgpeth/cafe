using System;

namespace cafe.CommandLine
{
    public class CommandOptionValueSpecification : OptionValueSpecification
    {
        private readonly string _command;

        public CommandOptionValueSpecification(string command, string description, bool isRequired = true)
            : base(description, isRequired)
        {
            _command = command;
        }

        public override bool IsSatisfiedBy(int position, params Argument[] args)
        {
            var commandArgument = args[position] as CommandArgument;
            return commandArgument != null && string.Equals(_command, commandArgument.Command,
                       StringComparison.OrdinalIgnoreCase);
        }

        public override Argument ParseArgument(string label, string value)
        {
            return string.Equals(_command, value) ? new CommandArgument(_command) : null;
        }
    }
}