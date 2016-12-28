using System;
using System.IO;
using cafe.CommandLine;
using Newtonsoft.Json;
using NLog;

namespace cafe
{
    public class ServerSettings
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(ServerSettings).FullName);


        public int ChefInterval { get; set; }
        public int Port { get; set; } = 59320;

        private static ServerSettings _instance;

        public static ServerSettings Read()
        {
            if (_instance == null)
            {
                const string serverJson = "server.json";
                if (File.Exists(serverJson))
                {
                    try
                    {
                        _instance = JsonConvert.DeserializeObject<ServerSettings>(File.ReadAllText(serverJson));
                        Logger.Info(
                            $"Server settings read as Chef Interval: {_instance.ChefInterval}, Port: {_instance.Port}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, $"Error when parsing {serverJson}");
                        Presenter.ShowError($"Could not parse settings file {serverJson}, so going with default settings",
                            Logger);
                        _instance = new ServerSettings();
                    }
                }
                else
                {
                    Presenter.ShowMessage($"Could not find {serverJson} so going with default settings", Logger);
                    _instance = new ServerSettings();
                }
            }
            return _instance;
        }
    }
}