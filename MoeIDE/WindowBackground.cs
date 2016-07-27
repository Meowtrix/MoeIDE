using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Meowtrix.MoeIDE
{
    public class WindowBackground
    {
        private readonly Image imagecontrol = new Image();
        public WindowBackground(Window window)
        {
            if (window.IsLoaded) Window_Loaded(window, null);
            window.Loaded += Window_Loaded;
            SettingsManager.SettingsUpdated += SettingsUpdated;
        }

        private void SettingsUpdated(SettingsModel oldSettings, SettingsModel newSettings)
        {
            try
            {
                var imagesource = BitmapFrame.Create(new Uri(newSettings.MainBackground.Filename), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                imagesource.Freeze();
                imagecontrol.Source = imagesource;
                imagecontrol.Stretch = newSettings.MainBackground.Stretch;
                imagecontrol.HorizontalAlignment = newSettings.MainBackground.HorizontalAlignment;
                imagecontrol.VerticalAlignment = newSettings.MainBackground.VerticalAlignment;
            }
            catch
            {
                imagecontrol.Source = null;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var mainwindow = (Window)sender;

            Grid.SetRowSpan(imagecontrol, 4);
            var rootgrid = (Grid)mainwindow.Template.FindName("RootGrid", mainwindow);
            rootgrid.Children.Insert(0, imagecontrol);
        }
    }
}
