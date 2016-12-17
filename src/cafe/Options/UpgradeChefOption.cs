using System;
using cafe.CommandLine;

namespace cafe.Options
{
    public class UpgradeChefOption : Option
    {
        public UpgradeChefOption()
            : base(
                new OptionSpecification(OptionValueSpecification.ForExactValue("chef"),
                    OptionValueSpecification.ForExactValue("upgrade"), OptionValueSpecification.ForVersion()),
                "upgrades chef to the specified version")
        {
        }

        protected override void RunCore(string[] args)
        {
            Console.Out.WriteLine("Running Upgrade Chef");
        }
    }
}