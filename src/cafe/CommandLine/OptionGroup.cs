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
            _groupSpecification = new OptionSpecification(groupSpecifications);
        }

        public OptionGroup(string group) : this(OptionValueSpecification.ForExactValue(group))
        {
        }

        public bool IsSatisfiedBy(params string[] args)
        {
            return _groupSpecification.IsSatisfiedBy(args) || _childOptions.Keys.Any(s => s.IsSatisfiedBy(args)) ||
                   _childGroups.Any(g => g.IsSatisfiedBy(args));
        }

        public KeyValuePair<OptionSpecification, Option> MatchingOptionPair(params string[] args)
        {
            return (from pair in _childOptions
                where pair.Key.IsSatisfiedBy(args) // || pair.Key.HelpRequested(args)
                select pair).FirstOrDefault();
        }

        public Option MatchingOption(params string[] args)
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
            return WithOption(option, args.Select(OptionValueSpecification.ForExactValue).ToArray());
        }

        public OptionGroup WithOption(Option option, params OptionValueSpecification[] valueSpecifications)
        {
            var optionSpecification = _groupSpecification.WithAdditionalSpecifications(valueSpecifications);
            _childOptions.Add(optionSpecification, option);
            return this;
        }


        public int RunProgram(params string[] args)
        {
            if (IsSatisfiedBy(args))
            {
                var result = Run(args);
                return result.IsSuccess ? 0 : -1;
            }
            Presenter.ShowError("No options match the supplied arguments. Run -h to view all options", Logger);
            return -2;
        }

        public Result Run(params string[] args)
        {
            if (args.Length == 0 || (_groupSpecification.IsSatisfiedBy(args) && _groupSpecification.HelpRequested(args)))
            {
                ShowHelp();
                return Result.Successful();
            }
            var matchingGroup = _childGroups.FirstOrDefault(g => g.IsSatisfiedBy(args));
            if (matchingGroup != null)
            {
                return matchingGroup.Run(args);
            }
            var pair = MatchingOptionPair(args);
            if (pair.Key.HelpRequested(args))
            {
                ShowHelp(pair);
                return Result.Successful();
            }
            return pair.Value.Run(args);
        }

        public OptionGroup WithGroup(string groupValue, Action<OptionGroup> groupInitializer)
        {
            var specifications =
                new List<OptionValueSpecification>(_groupSpecification.ValueSpecifications)
                {
                    OptionValueSpecification.ForExactValue(groupValue)
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
                ShowHelp(optionPair);
            }
        }

        private static void ShowHelp(KeyValuePair<OptionSpecification, Option> optionPair)
        {
            var option = optionPair.Value;
            Presenter.ShowMessage($"{optionPair.Key} <- {option}", Logger);
            option.NotifyHelpWasShown();
        }
    }
}