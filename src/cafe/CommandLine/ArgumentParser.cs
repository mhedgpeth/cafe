using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Semantics;

namespace cafe.CommandLine
{
    public class Argument
    {

    }

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
    }

    public class CommandArgument : Argument
    {
        private readonly string _command;

        public CommandArgument(string command)
        {
            _command = command;
        }

        public string Command => _command;
    }

    public class ArgumentParser
    {
        public static Argument[] Parse(params string[] args)
        {
            var arguments = new List<Argument>();
            string label = null;
            foreach (var arg in args)
            {
                if (arg.EndsWith(":"))
                {
                    label = arg;
                    continue;
                }
                else if (label != null)
                {
                    arguments.Add(new ValueArgument(label, arg));
                    label = null;
                }
                else
                {
                    arguments.Add(new CommandArgument(arg));
                }
            }
            return arguments.ToArray();
        }
    }

    public static class ArgumentEnumerableExtensions
    {
        public static ValueArgument FindValueFromLabel(this IEnumerable<Argument> arguments, string label)
        {
            return arguments.OfType<ValueArgument>().FirstOrDefault(v => v.Label == label);
        }
    }
}