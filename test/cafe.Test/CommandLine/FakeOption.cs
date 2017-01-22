using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Test.CommandLine
{
    public class FakeOption : Option
    {
        public FakeOption(string helpText) : base(helpText)
        {
        }

        protected override Result RunCore(string[] args)
        {
            WasRun = true;
            return Result.Successful();
        }

        public override void NotifyHelpWasShown()
        {
            WasHelpShown = true;
        }


        public bool WasRun { get; private set; }
        public bool WasHelpShown { get; private set; }
        protected override string ToDescription(string[] args)
        {
            return "Fake Option";
        }
    }
}