using System;
using cafe.Chef;
using cafe.CommandLine;

namespace cafe.Options
{
    public class InstallChefOption : Option
    {
        private readonly ChefInstaller _chefInstaller;

        public InstallChefOption(ChefInstaller chefInstaller)
            : base(
                new OptionSpecification(OptionValueSpecification.ForExactValue("chef"),
                    OptionValueSpecification.ForAnyValues("install", "upgrade"), OptionValueSpecification.ForVersion()),
                "installs or upgrades chef to the specified version")
        {
            _chefInstaller = chefInstaller;
        }

        protected override void RunCore(string[] args)
        {
            _chefInstaller.InstallOrUpgrade(args[2]);
        }
    }
}