using System.Collections.Generic;
using System.Linq;

namespace cafe.CommandLine
{
    public static class ArgumentEnumerableExtensions
    {
        public static ValueArgument FindValueFromLabel(this IEnumerable<Argument> arguments, string label)
        {
            return arguments.OfType<ValueArgument>().FirstOrDefault(v => v.Label == label);
        }

        public static CommandArgument FindCommand(this IEnumerable<Argument> arguments, string command)
        {
            return arguments.OfType<CommandArgument>().FirstOrDefault(c => c.Command == command);
        }

        public static bool ContainsHelpRequest(this IEnumerable<Argument> args)
        {
            return FindCommand(args, "-h") != null;
        }

    }
}