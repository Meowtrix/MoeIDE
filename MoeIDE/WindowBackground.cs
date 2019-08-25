using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Shell;

namespace Meowtrix.MoeIDE
{
    public class WindowBackground
    {
        private Border parentBorder;
        private Image imagecontrol;
        public WindowBackground(Window window)
        {
            if (window.IsLoaded) Window_Loaded(window, null);
            window.Loaded += Window_Loaded;
            SettingsManager.SettingsUpdated += SettingsUpdated;
        }

#pragma warning disable VSTHRD100
        private async void SettingsUpdated(SettingsModel oldSettings, SettingsModel newSettings)
#pragma warning restore
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {
                var imagesource = BitmapFrame.Create(new Uri(newSettings.MainBackground.Filename), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                imagesource.Freeze();
                imagecontrol.Source = imagesource;
                imagecontrol.Stretch = newSettings.MainBackground.Stretch;
                imagecontrol.HorizontalAlignment = newSettings.MainBackground.HorizontalAlignment;
                imagecontrol.VerticalAlignment = newSettings.MainBackground.VerticalAlignment;
                var br = new SolidColorBrush(newSettings.MainBackground.BackColor);
                br.Freeze();
                parentBorder.Background = br;
                imagecontrol.Opacity = newSettings.MainBackground.Opacity;
                double blur = newSettings.MainBackground.Blur;
                if (blur == 0.0)
                    imagecontrol.Effect = null;
                else imagecontrol.Effect = new BlurEffect { Radius = blur };
            }
            catch
            {
                imagecontrol.Source = null;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var mainwindow = (Window)sender;

            imagecontrol = new Image();
            parentBorder = new Border
            {
                Child = imagecontrol
            };
            var cache = new BitmapCache { SnapsToDevicePixels = true };
            cache.Freeze();
            parentBorder.CacheMode = cache;
            Grid.SetRowSpan(parentBorder, 4);
            var rootgrid = (Grid)mainwindow.Template.FindName("RootGrid", mainwindow);
            rootgrid.Children.Insert(0, parentBorder);
        }
    }
}
