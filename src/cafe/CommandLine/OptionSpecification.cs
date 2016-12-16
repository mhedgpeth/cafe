using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace cafe.CommandLine
{
    public class OptionSpecification
    {
        private static ILogger Logger { get; } =
          ApplicationLogging.CreateLogger<OptionSpecification>();

        private readonly string _description = string.Empty;
        private readonly OptionValueSpecification[] _valueSpecifications;

        public OptionSpecification(params string[] exactValues)
            : this(ConvertToExactValueSpecifications(exactValues))
        {

        }

        private static OptionValueSpecification[] ConvertToExactValueSpecifications(params string[] exactValues)
        {
            return exactValues.Select(OptionValueSpecification.ForExactValue).ToArray();
        }

        public OptionSpecification(params OptionValueSpecification[] valueSpecifications)
        {
            _valueSpecifications = valueSpecifications;
            foreach (var valueSpecification in _valueSpecifications)
            {
                _description += valueSpecification + " ";
            }
            _description = _description.TrimEnd();
        }

        public bool IsSatisfiedBy(params string[] args)
        {
            var trimmedArguments = TrimLastArgumentIfItIsHelpRequest(args);
            if (_valueSpecifications.Length == trimmedArguments.Length)
            {
                for (int i = 0; i < _valueSpecifications.Length; i++)
                {
                    var valueSpecification = _valueSpecifications[i];
                    var value = trimmedArguments[i];
                    Logger.LogDebug($"Determining if {value} matches specification {valueSpecification}");
                    if (!valueSpecification.IsSatisfiedBy(value))
                    {
                        Logger.LogDebug($"Since {value} is not satisfied by {valueSpecification}, {this} is not an option");
                        return false;
                    }
                }
                Logger.LogDebug($"Since all specifications match, {this} is an option");
                return true;
            }
            else
            {
                Logger.LogDebug($"Argument count doesn't match, so {this} isn't an option");
                return false;
            }
        }

        private string[] TrimLastArgumentIfItIsHelpRequest(string[] args)
        {
            if (!HelpRequested(args)) return args;
            var list = args.ToList();
            list.Remove("-h");
            return list.ToArray();
        }

        public override string ToString()
        {
            return _description;
        }

        public bool HelpRequested(string[] args)
        {
            return args.LastOrDefault() == "-h";
        }
    }
}