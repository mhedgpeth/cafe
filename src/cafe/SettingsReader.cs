using System;
using System.IO;
using cafe.CommandLine;
using Newtonsoft.Json;
using NLog;

namespace cafe
{
    public class SettingsReader
    {
        private static readonly Logger Logger = LogManager.GetLogger(typeof(SettingsReader).FullName);
        public static T Read<T>(string type, string file) where T : new()
        {
            var returnValue = new T();
            if (File.Exists(file))
            {
                try
                {
                    returnValue = JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
                    Logger.Info(
                        $"{type} settings read as {returnValue}");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Error when parsing {file} for {type} Settings");
                    Presenter.ShowError($"Could not parse settings file {file}, so going with default {type} settings",
                        Logger);
                }
            }
            else
            {
                Presenter.ShowMessage($"Could not find {file} so going with default {type} settings", Logger);
            }
            return returnValue;
        }

    }
}