using System;
using System.IO;
using System.Xml.Serialization;

namespace Meowtrix.MoeIDE
{
    internal delegate void SettingsUpdatedHandler(SettingsModel oldSettings, SettingsModel newSettings);

    internal static class SettingsManager
    {
        public static SettingsModel CurrentSettings { get; private set; } = new SettingsModel();
        public static event SettingsUpdatedHandler SettingsUpdated = delegate { };
        private static readonly string configFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(Meowtrix), nameof(MoeIDE));
        private static readonly FileSystemWatcher watcher;
        private const string filename = "userconfig.xml";
        static SettingsManager()
        {
            watcher = new FileSystemWatcher(configFolder, filename);
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Changed;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.Name != filename) return;
            LoadSettings();
        }

        public static void LoadSettings()
        {
            SettingsModel settings;
            try
            {
                var serialzer = new XmlSerializer(typeof(SettingsModel));
                using (var stream = File.OpenRead(Path.Combine(configFolder, filename)))
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
                Directory.CreateDirectory(configFolder);
                using (var stream = File.Create(Path.Combine(configFolder, filename)))
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
