using cafe.CommandLine;
using cafe.LocalSystem;
using cafe.Shared;
using NLog;

namespace cafe.Options
{
    public class InitOption : Option
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(InitOption).FullName);

        private readonly string _cafeDirectory;
        private readonly IEnvironment _environment;

        public InitOption(string cafeDirectory, IEnvironment environment) : base(
            "initializes cafe to run on this machine")
        {
            _cafeDirectory = cafeDirectory;
            _environment = environment;
        }

        protected override string ToDescription(Argument[] args)
        {
            return "Initializing Cafe to Run on This Machine";
        }

        public const string PathEnvironmentVariableKey = "PATH";

        protected override Result RunCore(Argument[] args)
        {
            var path = GetPathEnvironmentVariable();
            if (!path.Contains(_cafeDirectory))
            {
                Presenter.ShowMessage("Adding Cafe to path environment variable so it can run from anywhere", Logger);
                Logger.Info($"Path does not contain cafe directory {_cafeDirectory}, so adding it");
                path += $";{_cafeDirectory}";
                _environment.SetSystemEnvironmentVariable(PathEnvironmentVariableKey, path);
                Logger.Debug($"After updating path, its value is now {GetPathEnvironmentVariable()}");
                Presenter.ShowMessage("You'll need to reboot for these changes to be in effect", Logger);
            }
            else
            {
                Presenter.ShowMessage("Cafe is already on the PATH environment variable", Logger);
            }
            return Result.Successful();
        }

        private string GetPathEnvironmentVariable()
        {
            var path = _environment.GetEnvironmentVariable(PathEnvironmentVariableKey);
            return path;
        }
    }
}