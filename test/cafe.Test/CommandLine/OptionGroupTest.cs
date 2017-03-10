using System.Linq;
using cafe.CommandLine;
using FluentAssertions;
using Xunit;

namespace cafe.Test.CommandLine
{
    public static class OptionGroupTestExtensions
    {
        public static bool IsSatsifiedBy(this OptionGroup optionGroup, params string[] args)
        {
            var arguments = args.ToList().Select(a => new CommandArgument(a)).ToArray();
            return optionGroup.IsSatisfiedBy(arguments);
        }
    }

    public class OptionGroupTest
    {
        public static Argument[] ToCommandArguments(params string[] args)
        {
            return args.ToList().Select(a => new CommandArgument(a)).ToArray();
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeTrueForMatchingArguments()
        {
            FakeOption versionOption;
            FakeOption statusOption;
            var optionGroup = CreateGroupWithOption(out versionOption, out statusOption);

            optionGroup.IsSatsifiedBy("inspec", "version")
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

        private static bool IsSatisfiedBy(OptionGroup group, params string[] commands)
        {
            var arguments = commands.Select(Argument.CreateCommand).ToArray();
            return group.IsSatisfiedBy(arguments);
        }

        [Fact]
        public void IsSatisfiedBy_ShouldBeFalseForInvalidArguments()
        {
            FakeOption versionOption;
            FakeOption statusOption;
            var optionGroup = CreateGroupWithOption(out versionOption, out statusOption);

            IsSatisfiedBy(optionGroup, "inspec", "something")
                .Should()
                .BeFalse("because even though the group matches the option doesn't");
        }

        [Fact]
        public void MatchingOption_ShouldBeOptionWhenMatching()
        {
            FakeOption versionOption;
            FakeOption statusOption;
            var optionGroup = CreateGroupWithOption(out versionOption, out statusOption);

            optionGroup.MatchingOption(ToCommandArguments("inspec", "version"))
                .Should()
                .BeSameAs(versionOption);
        }

        [Fact]
        public void MatchingOption_ShouldBeNullWhenNotMatching()
        {
            FakeOption versionOption;
            FakeOption statusOption;
            var optionGroup = CreateGroupWithOption(out versionOption, out statusOption);

            optionGroup.MatchingOption(ToCommandArguments("inspec", "something")).Should().BeNull("because it's not in the group");
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

            root.IsSatisfiedBy(ToCommandArguments("inspec", "install")).Should().BeTrue("because the child group should be used");
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

            group.RunProgram(group.ParseArguments("chef", "run"));

            chefRunOption.WasRun.Should().BeTrue();
        }


        [Fact]
        public void Parse_ShouldSelectSecondOptionIfFirstDoesNotMatch()
        {
            FakeOption chefRunOption;
            FakeOption chefVersionOption;
            var group = CreateChefRunOption(out chefRunOption, out chefVersionOption);

            group.RunProgram(group.ParseArguments("chef", "version"));

            chefRunOption.WasRun.Should().BeFalse();
            chefVersionOption.WasRun.Should().BeTrue();
        }

        [Fact]
        public void ParseArguments_ShouldReturnNullWhenRequiredLabelArgumentIsNotThere()
        {
            var root = CreateChefDownloadGroup();

            var arguments = root.ParseArguments("chef", "download");

            arguments.Should().BeNull("because the version wasn't supplied");
        }

        [Fact]
        public void ParseArguments_ShouldReturnNullWhenTooManyArgumentsAreGiven()
        {
            var root = CreateChefDownloadGroup();

            var arguments = root.ParseArguments("chef", "download", "version:", "1.2.3", "another:", "something");

            arguments.Should().BeNull("because too many were supplied");
        }

        private static OptionGroup CreateChefDownloadGroup()
        {
            var download = new FakeOption("download chef");
            var root = new OptionGroup().WithGroup("chef",
                chefGroup => chefGroup.WithOption(download, OptionValueSpecification.ForCommand("download"),
                    OptionValueSpecification.ForVersion()));
            return root;
        }
    }
}