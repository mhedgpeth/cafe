using cafe.CommandLine;

namespace cafe.Options
{
    public class ServerOption : Option
    {
        public ServerOption() : base(new OptionSpecification("server"), "Starts cafe in server mode")
        {

        }

        protected override void RunCore(string[] args)
        {
        }
    }
}