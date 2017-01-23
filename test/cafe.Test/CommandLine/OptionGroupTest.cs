using cafe.CommandLine;
using cafe.Test.CommandLine;
using FluentAssertions;
using Xunit;

namespace cafe.Test.Options
{
    public class OptionGroupTest
    {
        [Fact]
        public void IsSatisfiedBy_ShouldBeTrueForMatchingArguments()
        {
            FakeOption versionOption;
            FakeOption statusOption;
            var optionGroup = CreateGroupWithOption(out versionOption, out statusOption);

            optionGroup.IsSatisfiedBy("inspec", "version")
                .Should()
                .BeTrue("because the argument matches the group, then the option");
        }

        private static OptionGroup CreateGroupWithOption(out FakeOption versionOption, out FakeOption statusOption)
        {
            versionOption = new FakeOption("version");
            statusOption = new FakeOption("status");
            var optionGroup = new OptionGroup("inspec")
                .WithOption(versionOption, "version")
                .WithOption(statusOption, "status");
            return optionGroup;
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeFalseForInvalidArguments()
        {
            FakeOption versionOption;
            FakeOption statusOption;
            var optionGroup = CreateGroupWithOption(out versionOption, out statusOption);

            optionGroup.IsSatisfiedBy("inspec", "something")
                .Should()
                .BeFalse("because even though the group matches the option doesn't");
        }

        [Fact]
        public void MatchingOption_ShouldBeOptionWhenMatching()
        {
            FakeOption versionOption;
            FakeOption statusOption;
            var optionGroup = CreateGroupWithOption(out versionOption, out statusOption);

            optionGroup.MatchingOption("inspec", "version")
                .Should()
                .BeSameAs(versionOption);
        }

        [Fact]
        public void MatchingOption_ShouldBeNullWhenNotMatching()
        {
            FakeOption versionOption;
            FakeOption statusOption;
            var optionGroup = CreateGroupWithOption(out versionOption, out statusOption);

            optionGroup.MatchingOption("inspec", "something").Should().BeNull("because it's not in the group");
        }

        [Fact]
        public void IsSatisfied_ShouldBeTrueForSubgroupOption()
        {
            var root = new OptionGroup()
                .WithGroup("inspec", inspecGroup =>
                {
                    inspecGroup.WithOption(new FakeOption("install"), "install");
                    inspecGroup.WithOption(new FakeOption("status"), "status");
                });

            root.IsSatisfiedBy("inspec", "install").Should().BeTrue("because the child group should be used");
        }

        [Fact]
        public void HelpArguments_ShouldShowHelp()
        {
            FakeOption versionOption;
            FakeOption statusOption;
            var optionGroup = CreateGroupWithOption(out versionOption, out statusOption);
            var inspecHelpArguments = new[] {"inspec", "-h"};

            bool isSatisfied = optionGroup.IsSatisfiedBy(inspecHelpArguments);
            var result = optionGroup.Run(inspecHelpArguments);

            isSatisfied.Should().BeTrue();
            result.IsSuccess.Should().BeTrue();
            result.IsSuccess.Should().BeTrue("because help was requested; that's legitimate");
            versionOption.WasHelpShown.Should().BeTrue("because help was requested for the group");
            statusOption.WasHelpShown.Should().BeTrue("because help was requested for the group");
        }

        [Fact]
        public void HelpArgumentInGroup_ShouldOnlyShowHelpForThatGroup()
        {
            var inspecVersion = new FakeOption("inspec version");
            var chefVersion = new FakeOption("chef version");
            var root = new OptionGroup()
                .WithGroup("inspec", i => i.WithOption(inspecVersion, "version"))
                .WithGroup("chef", c => c.WithOption(chefVersion, "version"));

            var inspecHelpArguments = new[] {"inspec", "-h"};

            bool isSatisfied = root.IsSatisfiedBy(inspecHelpArguments);
            var result = root.Run("inspec", "-h");

            isSatisfied.Should().BeTrue();
            result.IsSuccess.Should().BeTrue();
            inspecVersion.WasHelpShown.Should().BeTrue("because inspec group help was requested");
            chefVersion.WasHelpShown.Should().BeFalse("because chef group help was not requested");
        }

        private OptionGroup CreateChefRunOption(out FakeOption chefRunOption, out FakeOption chefVersionOption)
        {
            chefRunOption = new FakeOption("run chef");
            chefVersionOption = new FakeOption("shows chef version");
            var chefRunOptionCopy = chefRunOption;
            var chefVersionOptionCopy = chefVersionOption;
            return new OptionGroup()
                .WithGroup("chef", chefGroup =>
                {
                    chefGroup.WithOption(chefRunOptionCopy, "run");
                    chefGroup.WithOption(chefVersionOptionCopy, "version");
                });
        }

        [Fact]
        public void Parse_ShouldSelectMatchingOption()
        {
            FakeOption chefRunOption;
            FakeOption chefVersionOption;
            var group = CreateChefRunOption(out chefRunOption, out chefVersionOption);

            group.RunProgram("chef", "run");

            chefRunOption.WasRun.Should().BeTrue();
        }


        [Fact]
        public void Parse_ShouldSelectSecondOptionIfFirstDoesNotMatch()
        {
            FakeOption chefRunOption;
            FakeOption chefVersionOption;
            var group = CreateChefRunOption(out chefRunOption, out chefVersionOption);

            group.RunProgram("chef", "version");

            chefRunOption.WasRun.Should().BeFalse();
            chefVersionOption.WasRun.Should().BeTrue();
        }

        [Fact]
        public void Parse_ShouldMatchIfHelpIsAskedFor()
        {
            FakeOption chefRunOption;
            FakeOption chefVersionOption;
            var group = CreateChefRunOption(out chefRunOption, out chefVersionOption);

            group.RunProgram("chef", "run", "-h");

            chefRunOption.WasRun.Should().BeFalse();
            chefRunOption.WasHelpShown.Should().BeTrue();
        }

        [Fact]
        public void DashH_ShouldShowAllOptions()
        {
            AssertArgumentsShouldShowHelpOnAllOptions("-h");
        }

        private void AssertArgumentsShouldShowHelpOnAllOptions(params string[] args)
        {
            FakeOption chefRunOption;
            FakeOption chefVersionOption;
            var group = CreateChefRunOption(out chefRunOption, out chefVersionOption);

            @group.RunProgram(args);

            chefRunOption.WasHelpShown.Should().BeTrue("because help was asked for all options");
            chefVersionOption.WasHelpShown.Should().BeTrue("because help was asked for all options");
        }

        [Fact]
        public void NoArguments_ShouldShowHelp()
        {
            AssertArgumentsShouldShowHelpOnAllOptions();
        }

    }
}