using cafe.Shared;
using NLog;

namespace cafe.CommandLine
{
    public class HelpOption : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(OptionGroup).FullName);

        private readonly OptionGroup _optionGroup;
        private readonly Option _option;
        private readonly OptionSpecification _specification;

        public HelpOption(OptionGroup optionGroup, Option option = null,
            OptionSpecification specification = null) : base("help", false)
        {
            _optionGroup = optionGroup;
            _option = option;
            _specification = specification;
        }

        protected override string ToDescription(Argument[] args)
        {
            return $"Shows help for {_optionGroup} and {_option}";
        }

        protected override Result RunCore(Argument[] args)
        {
            if (_option != null)
            {
                ShowOptionHelp();
            }
            else
            {
                _optionGroup.ShowHelp();
            }
            return Result.Successful();
        }

        public void ShowOptionHelp()
        {
            Presenter.ShowMessage($"{_specification} <- {_option}", Logger);
            _option.NotifyHelpWasShown();
        }
    }
}