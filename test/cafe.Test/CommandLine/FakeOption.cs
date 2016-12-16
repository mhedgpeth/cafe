using cafe.CommandLine;

namespace cafe.Test.CommandLine
{
    public class FakeOption : Option
    {
        public FakeOption(OptionSpecification specification, string helpText) : base(specification, helpText)
        {
        }

        protected override void RunCore()
        {
            WasRun = true;
        }

        public override void ShowHelp()
        {
            WasHelpShown = true;
        }


        public bool WasRun { get; private set; }
        public bool WasHelpShown { get; private set; }
    }
}