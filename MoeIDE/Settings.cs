using System.ComponentModel;
using System.Drawing.Design;
using System.Windows;
using System.Windows.Forms.Design;
using System.Windows.Media;
using Meowtrix.MoeIDE.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace Meowtrix.MoeIDE
{
    public sealed class Settings : DialogPage
    {
        [LocalizedCategory("201"), LocalizedDescription("203"), LocalizedDisplayName("202"), Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string MainBackgroundFilename { get; set; }
        [LocalizedCategory("201"), LocalizedDescription("204"), DisplayName(nameof(Stretch)), DefaultValue(Stretch.UniformToFill)]
        public Stretch MainBackgroundStretch { get; set; } = Stretch.UniformToFill;
        [LocalizedCategory("201"), LocalizedDescription("205"), DisplayName(nameof(HorizontalAlignment)), DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment MainBackgroundHorizontalAlignment { get; set; } = HorizontalAlignment.Center;
        [LocalizedCategory("201"), LocalizedDescription("206"), DisplayName(nameof(VerticalAlignment)), DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment MainBackgroundVerticalAlignment { get; set; } = VerticalAlignment.Center;
        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                SettingsManager.SaveSettings(new MoeIDE.SettingsModel(this));
            }
        }
    }
}
