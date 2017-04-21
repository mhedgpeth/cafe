using cafe.LocalSystem;
using NLog;

namespace cafe.Chef
{
    public class BootstrapChefPolicy : RunChefPolicy
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(BootstrapChefPolicy).FullName);

        private readonly IFileSystemCommands _fileSystemCommands;
        private readonly string _config;
        private readonly string _validator;
        private readonly BootstrapSettings _bootstrapSettings;

        public BootstrapChefPolicy(IFileSystemCommands fileSystemCommands, string config, string validator,
            BootstrapSettings bootstrapSettings)
        {
            _fileSystemCommands = fileSystemCommands;
            _config = config;
            _validator = validator;
            _bootstrapSettings = bootstrapSettings;
        }


        private static readonly string ValidatorKeyPath = $@"{ChefInstallDirectory}\validator.pem";
        private static readonly string FirstRunFilename = $@"{ChefInstallDirectory}\first_run.json";

        public override void PrepareEnvironmentForChefRun()
        {
            Logger.Info("Preparing chef environment for the first run");
            WriteFirstRunFile();
            WriteServerConfig();
            WriteValidatorKey();
        }

        private void WriteValidatorKey()
        {
            Logger.Debug($"Writing to {ValidatorKeyPath}");
            _fileSystemCommands.WriteFileText(ValidatorKeyPath, _validator);
        }

        private void WriteServerConfig()
        {
            Logger.Debug($"Writing to {ClientConfigPath} {_config}");
            _fileSystemCommands.WriteFileText(ClientConfigPath, _config);
        }

        private void WriteFirstRunFile()
        {
            Logger.Debug($"Writing {_bootstrapSettings} to {FirstRunFilename}");
            _fileSystemCommands.WriteFileText(FirstRunFilename, _bootstrapSettings.ToJson());
        }

        protected override string[] AdditionalArgumentsForChefRun()
        {
            return new[] {"--json-attributes", FirstRunFilename};
        }

        public override string ToString()
        {
            return $"for the first time with {_bootstrapSettings}";
        }
    }
}