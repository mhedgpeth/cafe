using cafe.Chef;
using cafe.CommandLine;

namespace cafe.Options
{
    public class RunChefOption : Option
    {
        private readonly ChefRunner _chefRunner;

        public RunChefOption(ChefRunner chefRunner)
            : base(new OptionSpecification("chef", "run"), "runs chef")
        {
            _chefRunner = chefRunner;
        }

        protected override void RunCore(string[] args)
        {
            _chefRunner.Run();
        }
    }
}