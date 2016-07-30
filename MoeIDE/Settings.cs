using System;
using System.ComponentModel;
using System.Windows;
using Microsoft.VisualStudio.Shell;

namespace Meowtrix.MoeIDE
{
    public sealed class Settings : UIElementDialogPage
    {
        public SettingsModel Model { get; private set; }
        private readonly SettingsPage page = new SettingsPage();
        protected override UIElement Child => page;
        protected override void OnActivate(CancelEventArgs e)
        {
            if (Model == null)
            {
                Model = SettingsManager.CurrentSettings.Clone();
                page.DataContext = Model;
            }
            base.OnActivate(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            Model = null;
            base.OnClosed(e);
        }
        protected override void OnApply(PageApplyEventArgs e)
        {
            SettingsManager.SaveSettings(Model);
        }
    }
}
