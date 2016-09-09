using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Meowtrix.WPF.Extend;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;

namespace Meowtrix.MoeIDE
{
    /// <summary>
    /// Adornment class that draws a square box in the top right hand corner of the viewport
    /// </summary>
    public sealed class EditorBackground
    {
        /// <summary>
        /// Text view to add the adornment on.
        /// </summary>
        private readonly IWpfTextView view;

        private ContentControl control;
        private Grid parentGrid;
        private Canvas viewStack;
        private Grid leftMargin;
        private VisualBrush viewStackBrush;

        private RECT hostRect;
        private Panel hostRootVisual;
        private VisualBrush hostVisualBrush;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorBackground"/> class.
        /// </summary>
        /// <param name="view">The <see cref="IWpfTextView"/> upon which the adornment will be drawn</param>
        public EditorBackground(IWpfTextView view)
        {
            this.view = view;
            control = (ContentControl)view;
            control.Loaded += TextView_Loaded;
            view.BackgroundBrushChanged += TextView_BackgroundChanged;
            view.Closed += TextView_Closed;
        }

        private void TextView_BackgroundChanged(object sender, BackgroundBrushChangedEventArgs e)
            => control.Dispatcher.InvokeAsync(MakeBackgroundTransparent, DispatcherPriority.Render);

        private void TextView_Closed(object sender, EventArgs e)
        {
            VSColorTheme.ThemeChanged -= SetSolidBrush;
            Application.Current.MainWindow.SizeChanged -= SetVisualBrush;
            view.BackgroundBrushChanged -= TextView_BackgroundChanged;
            view.Closed -= TextView_Closed;
        }

        private void TextView_Loaded(object sender, RoutedEventArgs e)
        {
            if (parentGrid == null) parentGrid = control.Parent as Grid;
            if (viewStack == null) viewStack = control.Content as Canvas;
            if (leftMargin == null) leftMargin = (parentGrid.BFS().FirstOrDefault(x => x.GetType().Name == "LeftMargin") as Grid)?.Children[0] as Grid;

            if (!control.IsDescendantOf(Application.Current.MainWindow))
            {
                var source = PresentationSource.FromVisual(control) as HwndSource;
                hostRootVisual = source.RootVisual as Panel;
                if (hostRootVisual?.GetType().Name == "WpfMultiViewHost")//xaml editor
                {
                    source.AddHook(WndHook);

                    var containerBorder = new Border();
                    source.RootVisual = containerBorder;
                    containerBorder.Child = hostRootVisual;

                    VSColorTheme.ThemeChanged += SetSolidBrush;
                    SetSolidBrush(null);

                    var mainWindow = Application.Current.MainWindow;
                    hostVisualBrush = new VisualBrush(((Grid)mainWindow.Template.FindName("RootGrid", mainWindow)).Children[0]);
                    containerBorder.Background = hostVisualBrush;
                    mainWindow.SizeChanged += SetVisualBrush;
                }
                else
                {
                    view.BackgroundBrushChanged -= TextView_BackgroundChanged;
                    return;
                }
            }

            viewStackBrush = new VisualBrush(parentGrid);
            MakeBackgroundTransparent();
        }

        private IntPtr WndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            RECT rect;
            NativeMethods.GetWindowRect(hwnd, out rect);
            if (hostRect.Left != rect.Left ||
                hostRect.Right != rect.Right ||
                hostRect.Top != rect.Top ||
                hostRect.Bottom != rect.Bottom)
            {
                hostRect = rect;
                SetVisualBrush(null, null);
            }
            return IntPtr.Zero;
        }

        private void SetSolidBrush(ThemeChangedEventArgs e)
        {
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell6;
            var color = uiShell.GetThemedWPFColor(EnvironmentColors.SystemWindowColorKey);
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            hostRootVisual.Background = brush;
        }

        private void SetVisualBrush(object sender, SizeChangedEventArgs e)
        {
            RECT mainRect;
            NativeMethods.GetWindowRect(((HwndSource)PresentationSource.FromVisual(Application.Current.MainWindow)).Handle,
                out mainRect);
            double x = (hostRect.Left - mainRect.Left) / (double)mainRect.Width,
                y = (hostRect.Top - mainRect.Top) / (double)mainRect.Height,
                width = hostRect.Width / (double)mainRect.Width,
                height = hostRect.Height / (double)mainRect.Height;
            if (x < 0 || y < 0 || width > 1 || height > 1) return;
            hostVisualBrush.Viewbox = new Rect(x, y, width, height);
        }

        private void MakeBackgroundTransparent()
        {
            if (parentGrid != null) parentGrid.ClearValue(Panel.BackgroundProperty);
            if (viewStack != null) viewStack.Background = viewStackBrush;
            if (leftMargin != null) leftMargin.ClearValue(Panel.BackgroundProperty);
        }
    }
}
