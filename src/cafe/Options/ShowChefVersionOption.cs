using cafe.Chef;
using cafe.CommandLine;
using Microsoft.Extensions.Logging;

namespace cafe.Options
{
    public class ShowChefVersionOption : Option
    {
        private static ILogger Logger { get; } =
          ApplicationLogging.CreateLogger<ShowChefVersionOption>();

        private readonly ChefRunner _chefRunner;

        public ShowChefVersionOption(ChefRunner chefRunner)
        : base(new OptionSpecification("chef", "version"), "show the current version of chef")
        {
            _chefRunner = chefRunner;
        }

        protected override void RunCore(string[] args)
        {
            var version = _chefRunner.RetrieveVersion();
            Logger.LogInformation($"chef-client version: {version}");
        }
    }
}