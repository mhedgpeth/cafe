using cafe.CommandLine;

namespace cafe.Updater
{
    public class UpdaterSettings
    {
        public string CafeApplicationDirectory { get; set; } = "..";

        public static readonly UpdaterSettings Instance = SettingsReader.Read<UpdaterSettings>("Updater", "updater.json");

        public override string ToString()
        {
            return $"Updater of Cafe at {CafeApplicationDirectory}";
        }
    }
}