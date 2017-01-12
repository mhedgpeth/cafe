using cafe.LocalSystem;

namespace cafe.Chef
{
    public class ChefBootstrapper : IChefBootstrapper
    {
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


        private const string ChefInstallDirectory = @"C:\chef\client";
        private static readonly string ServerConfigPath = $@"#{ChefInstallDirectory}\client.rb";
        private static readonly string ValidatorKeyPath = $@"#{ChefInstallDirectory}\validator.pem";
        private static readonly string FirstRunFilename = $@"#{ChefInstallDirectory}\first_run.json";

        public void PrepareEnvironmentForChefRun()
        {
            WriteFirstRunFile(_bootstrapSettings);
            WriteServerConfig();
            WriteValidatorKey();
        }

        private void WriteValidatorKey()
        {
            _fileSystemCommands.WriteFileText(ValidatorKeyPath, _validator);
        }

        private void WriteServerConfig()
        {
            _fileSystemCommands.WriteFileText(ServerConfigPath, _config);
        }

        private void WriteFirstRunFile(BootstrapSettings bootstrapSettings)
        {
            _fileSystemCommands.WriteFileText(FirstRunFilename, bootstrapSettings.ToJson());
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