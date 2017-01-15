using cafe.LocalSystem;
using NLog;
using NLog.Fluent;

namespace cafe.Chef
{
    public class ChefBootstrapper : IChefBootstrapper
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ChefBootstrapper).FullName);

        private readonly IFileSystemCommands _fileSystemCommands;
        private readonly string _config;
        private readonly string _validator;
        private readonly BootstrapSettings _bootstrapSettings;

        public ChefBootstrapper(IFileSystemCommands fileSystemCommands, string config, string validator,
            BootstrapSettings bootstrapSettings)
        {
            _fileSystemCommands = fileSystemCommands;
            _config = config;
            _validator = validator;
            _bootstrapSettings = bootstrapSettings;
        }


        private const string ChefInstallDirectory = @"C:\chef";
        private static readonly string ServerConfigPath = $@"{ChefInstallDirectory}\client.rb";
        private static readonly string ValidatorKeyPath = $@"{ChefInstallDirectory}\validator.pem";
        private static readonly string FirstRunFilename = $@"{ChefInstallDirectory}\first_run.json";

        public void PrepareEnvironmentForChefRun()
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
            Logger.Debug($"Writing to {ServerConfigPath} {_config}");
            _fileSystemCommands.WriteFileText(ServerConfigPath, _config);
        }

        private void WriteFirstRunFile()
        {
            Logger.Debug($"Writing {_bootstrapSettings} to {FirstRunFilename}");
            _fileSystemCommands.WriteFileText(FirstRunFilename, _bootstrapSettings.ToJson());
        }

        public string[] ArgumentsForChefRun()
        {
            return new[] {"--json-attributes", FirstRunFilename};
        }

        public override string ToString()
        {
            return $"for the first time with {_bootstrapSettings}";
        }
    }
}