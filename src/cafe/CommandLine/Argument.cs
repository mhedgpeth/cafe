namespace cafe.CommandLine
{
    public abstract class Argument
    {
        public static Argument CreateCommand(string command)
        {
            return new CommandArgument(command);
        }
    }
}