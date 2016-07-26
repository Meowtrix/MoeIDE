using System;
using System.IO;
using System.Xml.Serialization;

namespace Meowtrix.MoeIDE
{
    internal delegate void SettingsUpdatedHandler(SettingsModel oldSettings, SettingsModel newSettings);

    internal static class SettingsManager
    {
        public static SettingsModel CurrentSettings { get; private set; } = new SettingsModel();
        public static event SettingsUpdatedHandler SettingsUpdated;
        private static string configFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Meowtrix", "MoeIDE", "userconfig.xml");
        public static void LoadSettings()
        {
            SettingsModel settings;
            try
            {
                var serialzer = new XmlSerializer(typeof(SettingsModel));
                using (var stream = File.OpenRead(configFilename))
                    settings = (SettingsModel)serialzer.Deserialize(stream);
                SettingsUpdated(CurrentSettings, settings);
                CurrentSettings = settings;
            }
            catch
            {
                //TODO:output
            }
        }
        public static void SaveSettings(SettingsModel settings)
        {
            try
            {
                var serialzer = new XmlSerializer(typeof(SettingsModel));
                Directory.CreateDirectory(Path.GetDirectoryName(configFilename));
                using (var stream = File.Create(configFilename))
                    serialzer.Serialize(stream, settings);
                SettingsUpdated(CurrentSettings, settings);
                CurrentSettings = settings;
            }
            catch
            {
                //TODO:output
            }
        }
    }
}
