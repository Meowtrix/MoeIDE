using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Meowtrix.MoeIDE.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace Meowtrix.MoeIDE
{
    public sealed class Settings : DialogPage
    {
        [LocalizedCategory("201"), LocalizedDescription("203"), LocalizedDisplayName("202"), Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string MainBackgroundFilename { get; set; }
        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                SettingsManager.SaveSettings(new MoeIDE.SettingsModel(this));
            }
        }
    }
}
