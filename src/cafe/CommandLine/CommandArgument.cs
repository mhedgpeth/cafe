namespace cafe.CommandLine
{
    public class CommandArgument : Argument
    {
        private readonly string _command;

        public CommandArgument(string command)
        {
            _command = command;
        }

        public string Command => _command;

        public override string ToString()
        {
            return _command;
        }
    }
}