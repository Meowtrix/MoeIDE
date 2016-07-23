using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowtrix.IDEBackground
{
    internal static class ConfigurationManager
    {
        private const string SectionName = "IDEBackground";
        private const string FileName = "user.config";
        public static readonly Type sectionType = typeof(ConfigurationSection);
        public static readonly string sectionFullName = typeof(ConfigurationSection).FullName;
        private static Configuration config;
        private static FileSystemWatcher watcher;
        public static string SettingsFolder { get; }
        public static ConfigurationSection Section { get; private set; }
        static ConfigurationManager()
        {
            SettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Meowtrix", SectionName);
            Directory.CreateDirectory(SettingsFolder);
            watcher = new FileSystemWatcher(SettingsFolder, FileName);
            watcher.Changed += ConfigureFileUpdated;
            watcher.Renamed += ConfigureFileUpdated;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
            RefreshConfiguration();
        }

        private static void RefreshConfiguration()
        {
            try
            {
                config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
                {
                    ExeConfigFilename = Path.Combine(SettingsFolder, "IDEBackground.config"),
                    RoamingUserConfigFilename = Path.Combine(SettingsFolder, FileName)
                }, ConfigurationUserLevel.PerUserRoaming);
                Section = config.Sections[sectionFullName] as ConfigurationSection;
                if (Section == null)
                {
                    Section = new ConfigurationSection();
                }
            }
            catch { }
        }

        private static void ConfigureFileUpdated(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
