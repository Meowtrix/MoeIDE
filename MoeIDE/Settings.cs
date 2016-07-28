using System;
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
        private bool initialized;
        [LocalizedCategory("201"), LocalizedDescription("203"), LocalizedDisplayName("202"), Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string MainBackgroundFilename { get; set; }
        [LocalizedCategory("201"), LocalizedDescription("204"), DisplayName(nameof(Stretch)), DefaultValue(Stretch.UniformToFill)]
        public Stretch MainBackgroundStretch { get; set; }
        [LocalizedCategory("201"), LocalizedDescription("205"), DisplayName(nameof(HorizontalAlignment)), DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment MainBackgroundHorizontalAlignment { get; set; }
        [LocalizedCategory("201"), LocalizedDescription("206"), DisplayName(nameof(VerticalAlignment)), DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment MainBackgroundVerticalAlignment { get; set; }
        [LocalizedCategory("201"), LocalizedDescription("208"), LocalizedDisplayName("207")]
        public Color MainBackColor { get; set; }
        [LocalizedCategory("201"), LocalizedDescription("210"), LocalizedDisplayName("209"), DefaultValue(1.0)]
        public double MainOpacity { get; set; }
        [LocalizedCategory("201"), LocalizedDescription("212"), LocalizedDisplayName("211"), DefaultValue(0.0)]
        public double MainBlur { get; set; }
        protected override void OnActivate(CancelEventArgs e)
        {
            if (!initialized)
            {
                var settings = SettingsManager.CurrentSettings;
                MainBackgroundFilename = settings.MainBackground.Filename;
                MainBackgroundStretch = settings.MainBackground.Stretch;
                MainBackgroundHorizontalAlignment = settings.MainBackground.HorizontalAlignment;
                MainBackgroundVerticalAlignment = settings.MainBackground.VerticalAlignment;
                MainBackColor = settings.MainBackground.BackColor;
                MainOpacity = settings.MainBackground.Opacity;
                MainBlur = settings.MainBackground.Blur;
                initialized = true;
            }
            base.OnActivate(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            initialized = false;
            base.OnClosed(e);
        }
        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                initialized = true;
                SettingsManager.SaveSettings(new MoeIDE.SettingsModel(this));
            }
        }
    }
}
