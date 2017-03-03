using System.Collections.Generic;
using System.Linq;
using NLog;

namespace cafe.CommandLine
{
    public class OptionSpecification
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(OptionSpecification).FullName);

        private readonly string _description = string.Empty;
        private readonly OptionValueSpecification[] _valueSpecifications;

        public OptionSpecification(params string[] exactValues)
            : this(ConvertToCommandSpecifications(exactValues))
        {
        }

        public OptionValueSpecification[] ValueSpecifications => _valueSpecifications;

        private static OptionValueSpecification[] ConvertToCommandSpecifications(params string[] exactValues)
        {
            return exactValues.Select(OptionValueSpecification.ForCommand).ToArray();
        }

        public OptionSpecification(params OptionValueSpecification[] valueSpecifications)
        {
            _valueSpecifications = valueSpecifications;
            foreach (var valueSpecification in _valueSpecifications.Where(v => v.IsRequired))
            {
                _description += valueSpecification + " ";
            }
            _description = _description.TrimEnd();
        }

        public bool IsSatisfiedBy(params Argument[] args)
        {
            if (_valueSpecifications.Length >= args.Length)
            {
                for (int i = 0; i < _valueSpecifications.Length; i++)
                {
                    var valueSpecification = _valueSpecifications[i];
                    if (i >= args.Length)
                    {
                        if (valueSpecification.IsRequired)
                        {
                            return false;
                        }
                        continue;
                    }
                    var value = args[i];
                    Logger.Debug($"Determining if {value} matches specification {valueSpecification}");
                    if (!valueSpecification.IsSatisfiedBy(i, args))
                    {
                        Logger.Debug(
                            $"Since {value} is not satisfied by {valueSpecification}, {this} is not an option");
                        return false;
                    }
                }
                Logger.Debug($"Since all specifications match, {this} is an option");
                return true;
            }
            else
            {
                Logger.Debug($"Argument count doesn't match, so {this} isn't an option");
                return false;
            }
        }

        public override string ToString()
        {
            return _description;
        }

        public OptionSpecification WithAdditionalSpecifications(params OptionValueSpecification[] valueSpecifications)
        {
            var allValues = new List<OptionValueSpecification>();
            allValues.AddRange(_valueSpecifications);
            allValues.RemoveAll(a => !a.IsRequired);
            allValues.AddRange(valueSpecifications);
            var optionSpecification = new OptionSpecification(allValues.ToArray());
            return optionSpecification;
        }

        public Argument[] ParseArguments(params string[] args)
        {
            var arguments = new List<Argument>();
            int currentSpecificationIndex = 0;
            var label = string.Empty;
            for (int i = 0; i < args.Length; i++)
            {
                var argument = args[i];
                if (argument.EndsWith(":"))
                {
                    label = argument;
                    continue;
                }
                if (currentSpecificationIndex >= _valueSpecifications.Length)
                {
                    return null;
                }
                var valueSpecification = _valueSpecifications[currentSpecificationIndex];
                var parsedArgument = valueSpecification.ParseArgument(label, argument);
                if (parsedArgument == null) return null;
                arguments.Add(parsedArgument);
                currentSpecificationIndex++;
            }
            return arguments.ToArray();
        }
    }
}