using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowtrix.MoeIDE
{
    internal static class MoeIDEConfigurationManager
    {
        private const string SectionName = nameof(MoeIDE);
        private const string FileName = "user.config";
        public static readonly Type sectionType = typeof(MoeIDEConfigurationSection);
        public static readonly string sectionFullName = typeof(MoeIDEConfigurationSection).FullName;
        private static Configuration config;
        private static readonly FileSystemWatcher watcher;
        public static string SettingsFolder { get; }
        public static MoeIDEConfigurationSection Section { get; private set; }
        static MoeIDEConfigurationManager()
        {
            SettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(Meowtrix), SectionName);
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
                config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
                {
                    ExeConfigFilename = Path.Combine(SettingsFolder, "MoeIDE.config"),
                    RoamingUserConfigFilename = Path.Combine(SettingsFolder, FileName)
                }, ConfigurationUserLevel.PerUserRoaming);
                Section = config.Sections[sectionFullName] as MoeIDEConfigurationSection;
                if (Section == null)
                {
                    Section = new MoeIDEConfigurationSection();
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
