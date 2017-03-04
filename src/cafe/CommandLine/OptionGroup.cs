using System;
using System.Collections.Generic;
using System.Linq;
using cafe.Shared;
using NLog;

namespace cafe.CommandLine
{
    public class OptionGroup
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(OptionGroup).FullName);

        private readonly OptionSpecification _groupSpecification;

        private readonly IDictionary<OptionSpecification, Option> _childOptions =
            new Dictionary<OptionSpecification, Option>();

        private readonly IList<OptionGroup> _childGroups = new List<OptionGroup>();

        public OptionGroup(params OptionValueSpecification[] groupSpecifications)
        {
            _groupSpecification =
                new OptionSpecification(groupSpecifications).WithAdditionalSpecifications(OptionValueSpecification
                    .OptionalHelpCommand());
        }

        public OptionGroup(string group) : this(OptionValueSpecification.ForCommand(group))
        {
        }

        public bool IsSatisfiedBy(params Argument[] args)
        {
            return _groupSpecification.IsSatisfiedBy(args) || _childOptions.Keys.Any(s => s.IsSatisfiedBy(args)) ||
                   _childGroups.Any(g => g.IsSatisfiedBy(args));
        }

        public KeyValuePair<OptionSpecification, Option> MatchingOptionPair(params Argument[] args)
        {
            return (from pair in _childOptions
                where pair.Key.IsSatisfiedBy(args) // || pair.Key.HelpRequested(args)
                select pair).FirstOrDefault();
        }

        public Option MatchingOption(params Argument[] args)
        {
            var pair = MatchingOptionPair(args);
            return pair.Equals(default(KeyValuePair<OptionSpecification, Option>)) ? null : pair.Value;
        }

        public OptionGroup WithDefaultOption(Option option)
        {
            return WithOption(option, new OptionValueSpecification[0]);
        }

        public OptionGroup WithOption(Option option, params string[] args)
        {
            return WithOption(option, args.Select(OptionValueSpecification.ForCommand).ToArray());
        }

        public OptionGroup WithOption(Option option, params OptionValueSpecification[] valueSpecifications)
        {
            var optionSpecification = _groupSpecification.WithAdditionalSpecifications(valueSpecifications)
                .WithAdditionalSpecifications(OptionValueSpecification.OptionalHelpCommand());
            _childOptions.Add(optionSpecification, option);
            return this;
        }


        public int RunProgram(params Argument[] arguments)
        {
            var result = Run(arguments);
            return result.IsSuccess ? 0 : -1;
        }

        public Argument[] ParseArguments(params string[] args)
        {
            var arguments = _groupSpecification.ParseArguments(args);
            if (arguments != null)
            {
                return arguments;
            }
            foreach (var childGroup in _childGroups)
            {
                var childArguments = childGroup.ParseArguments(args);
                if (childArguments != null)
                {
                    return childArguments;
                }
            }
            foreach (var childOptionSpecification in _childOptions.Keys)
            {
                var optionArguments = childOptionSpecification.ParseArguments(args);
                if (optionArguments != null)
                {
                    return optionArguments;
                }
            }
            return null;
        }

        public Option FindOption(params Argument[] args)
        {
            if (args.Length == 0 || (_groupSpecification.IsSatisfiedBy(args) &&
                                     args.ContainsHelpRequest()))
            {
                return new HelpOption(this);
            }
            var matchingGroup = _childGroups.FirstOrDefault(g => g.IsSatisfiedBy(args));
            if (matchingGroup != null)
            {
                return matchingGroup.FindOption(args);
            }
            var pair = MatchingOptionPair(args);
            if (args.ContainsHelpRequest())
            {
                return new HelpOption(this, pair.Value, pair.Key);
            }
            return pair.Value;
        }

        public Result Run(params Argument[] args)
        {
            var option = FindOption(args);
            return option.Run(args);
        }

        public OptionGroup WithGroup(string groupValue, Action<OptionGroup> groupInitializer)
        {
            var specifications =
                new List<OptionValueSpecification>(_groupSpecification.ValueSpecifications)
                {
                    OptionValueSpecification.ForCommand(groupValue)
                };
            var optionGroup = new OptionGroup(specifications.ToArray());
            groupInitializer(optionGroup);
            _childGroups.Add(optionGroup);
            return this;
        }

        public void ShowHelp()
        {
            if (_groupSpecification.ValueSpecifications.Length > 0)
            {
                Presenter.ShowMessage($"-- {_groupSpecification} --", Logger);
            }
            if (_childGroups.Count > 0)
            {
                Presenter.ShowMessage($"Showing help for {_childGroups.Count} groups:", Logger);
                foreach (var childGroup in _childGroups)
                {
                    childGroup.ShowHelp();
                    Presenter.NewLine();
                }

                Presenter.NewLine();
                Presenter.ShowMessage("-- other options --", Logger);
            }

            foreach (var optionPair in _childOptions)
            {
                var help = new HelpOption(this, optionPair.Value, optionPair.Key);
                help.Run();
            }
        }

        public override string ToString()
        {
            return $"{_groupSpecification} with {_childGroups.Count} groups and {_childOptions.Count} options";
        }
    }
}