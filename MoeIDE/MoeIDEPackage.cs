using System;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Meowtrix.MoeIDE.ComponentModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Meowtrix.MoeIDE
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [ProvideOptionPage(typeof(Settings), nameof(MoeIDE), "General", 0, 0, true)]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    public sealed class MoeIDEPackage : Package
    {
        /// <summary>
        /// MoeIDEPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "396fe64e-807b-43f4-a39b-0d7122c48f1a";

        static MoeIDEPackage()
        {
            var resman = new ResourceManager("VSPackage", typeof(MoeIDEPackage).Assembly);
            LocalizedCategoryAttribute.resources = resman;
            LocalizedDisplayNameAttribute.resources = resman;
            LocalizedDescriptionAttribute.resources = resman;
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Application.Current.MainWindow.Loaded += MainWindow_Loaded;
            SettingsManager.LoadSettings();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var mainwindow = (Window)sender;
            var imagesource = BitmapFrame.Create(new Uri(@"D:\WallPapers\aokana_misaki.png"), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            imagesource.Freeze();

            var imagecontrol = new Image
            {
                Source = imagesource,
                Stretch = Stretch.UniformToFill,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRowSpan(imagecontrol, 4);
            var rootgrid = (Grid)mainwindow.Template.FindName("RootGrid", mainwindow);
            rootgrid.Children.Insert(0, imagecontrol);
        }

        #endregion
    }
}
