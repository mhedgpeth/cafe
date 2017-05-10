using cafe.CommandLine;
using NLog;

namespace cafe
{
    public class ServerSettings
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ServerSettings).FullName);

        /// <summary>
        /// Gets or sets the Chef Interval
        /// </summary>
        /// <remarks>when this changes, the server does not need to be restarted to adjust to the change</remarks>
        public int ChefInterval { get; set; }
        /// <summary>
        // Gets or sets the port that the application listens on
        /// </summary>
        /// <remarks>when changing the port, you must restart the service</remarks>
        public int Port { get; set; } = DefaultPort;
        /// <summary>
        /// Gets or sets the folder that chef is installed on.
        /// </summary>
        /// <remarks>
        /// It is assumed that this is the folder cafe is installed on as well.
        /// Also, when this setting changes, the system will respond the next time chef is run.
        /// This should rarely if ever change, and only at installation.
        /// </remarks>
        public string InstallRoot { get; set; } = @"C:";
        /// <summary>
        /// Gets or sets the folder that the cafe.Updater is installed in
        /// </summary>
        /// <remarks>this should only change in a testing scenario</remarks>
        public string UpdaterDirectory { get; set; } = "updater";

        public const int DefaultPort = 59320;

        private static ServerSettings _instance;

        public static ServerSettings Instance => _instance ?? (_instance = SettingsReader.Read<ServerSettings>("Server", "server.json"));

        public static void Reload()
        {
            Logger.Debug("Reloading server settings the next time they are accessed");
            _instance = null;
        }

        public override string ToString()
        {
            return $"Chef Interval: {ChefInterval}, Port: {Port}";
        }
    }
}