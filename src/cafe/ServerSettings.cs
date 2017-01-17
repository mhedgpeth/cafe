namespace cafe
{
    public class ServerSettings
    {
        public int ChefInterval { get; set; }
        public int Port { get; set; } = DefaultPort;

        public const int DefaultPort = 59320;

        public static readonly ServerSettings Instance = SettingsReader.Read<ServerSettings>("Server", "server.json");

        public override string ToString()
        {
            return $"Chef Interval: {ChefInterval}, Port: {Port}";
        }
    }
}