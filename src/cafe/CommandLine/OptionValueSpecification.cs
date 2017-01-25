using System;
using System.Text.RegularExpressions;

namespace cafe.CommandLine
{
    public abstract class OptionValueSpecification
    {
        private readonly string _description;

        protected OptionValueSpecification(string description)
        {
            _description = description;
        }

        public abstract bool IsSatisfiedBy(int position, params string[] args);

        public static OptionValueSpecification ForVersion()
        {
            return new MatchingRegularExpressionLabelledValueSpecification("version:", new Regex(@"\d+\.\d+\.\d+"), "the version");
        }

        public override string ToString()
        {
            return _description;
        }

        public abstract Argument ParseArgument(int position, params string[] args);

        public static OptionValueSpecification ForCommand(string command)
        {
            return new CommandOptionValueSpecification(command, command);
        }

        public static OptionValueSpecification ForValue(string label, string description)
        {
            return new AnyLabelledValueSpecification(label, description);
        }
    }

    public class CommandOptionValueSpecification : OptionValueSpecification
    {
        private readonly string _command;

        public CommandOptionValueSpecification(string command, string description) : base(description)
        {
            _command = command;
        }

        public override bool IsSatisfiedBy(int position, params string[] args)
        {
            return string.Equals(_command, args[0], StringComparison.OrdinalIgnoreCase);
        }

        public override Argument ParseArgument(int position, params string[] args)
        {
            return args[position] == _command ? new CommandArgument(_command) : null;
        }
    }

    public class AnyLabelledValueSpecification : LabelledValueSpecification
    {
        public AnyLabelledValueSpecification(string label, string description) : base(label, description)
        {
        }

        protected override bool IsSatsifiedByValue(string value)
        {
            return true;
        }
    }

    public class MatchingRegularExpressionLabelledValueSpecification : LabelledValueSpecification
    {
        private readonly Regex _regularExpression;

        public MatchingRegularExpressionLabelledValueSpecification(string label, Regex regularExpression, string description) : base(label, description)
        {
            _regularExpression = regularExpression;
        }

        protected override bool IsSatsifiedByValue(string value)
        {
            return _regularExpression.IsMatch(value);
        }
    }

    public abstract class LabelledValueSpecification : OptionValueSpecification
    {
        private readonly string _label;

        protected LabelledValueSpecification(string label, string description) : base(description)
        {
            _label = label;
        }

        public override bool IsSatisfiedBy(int position, params string[] args)
        {
            return ParseArgument(position, args) != null;
        }

        public override Argument ParseArgument(int position, params string[] args)
        {
            int labelIndex = Array.IndexOf(args, _label);
            if (labelIndex >= 0)
            {
                if (labelIndex != args.Length - 1)
                {
                    var value = args[labelIndex + 1];
                    if (IsSatsifiedByValue(value))
                    {
                        return CreateValueArgument(value);
                    }
                }
                return null;
            }
            if (position < args.Length)
            {
                var value = args[position];
                if (IsSatsifiedByValue(value))
                {
                    return CreateValueArgument(args[position]);
                }
            }
            return null;
        }

        protected abstract bool IsSatsifiedByValue(string value);

        private ValueArgument CreateValueArgument(string value)
        {
            return new ValueArgument(_label, value);
        }
    }
}