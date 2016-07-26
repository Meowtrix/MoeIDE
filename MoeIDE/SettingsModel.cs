using System;

namespace Meowtrix.MoeIDE
{
    [Serializable]
    public class SettingsModel
    {
        public string MainBackgroundFilename { get; set; }
        public SettingsModel() { }
        public SettingsModel(Settings settings)
        {
            MainBackgroundFilename = settings.MainBackgroundFilename;
        }
    }
}
