using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
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

        private readonly ContentControl control;
        private Panel parentGrid;
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

        private async void TextView_BackgroundChanged(object sender, BackgroundBrushChangedEventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            MakeBackgroundTransparent();
        }

        private static IEnumerable<DependencyObject> BFS(DependencyObject root)
        {
            if (root == null) yield break;

            Queue<DependencyObject> queue = new Queue<DependencyObject>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                int childrenCount = VisualTreeHelper.GetChildrenCount(current);
                yield return current;
                for (int i = 0; i < childrenCount; i++)
                    queue.Enqueue(VisualTreeHelper.GetChild(current, i));
            }
        }

        private void TextView_Closed(object sender, EventArgs e)
        {
            VSColorTheme.ThemeChanged -= SetSolidBrush;
            Application.Current.MainWindow.SizeChanged -= SetVisualBrush;
            view.BackgroundBrushChanged -= TextView_BackgroundChanged;
            view.Closed -= TextView_Closed;
        }

        private void TextView_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (parentGrid == null) parentGrid = control.Parent as Panel;
            if (viewStack == null) viewStack = control.Content as Canvas;
            if (leftMargin == null) leftMargin = (BFS(parentGrid).FirstOrDefault(x => x.GetType().Name == "LeftMargin") as Panel)?.Children[0] as Grid;

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
            NativeMethods.GetWindowRect(hwnd, out RECT rect);
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
            ThreadHelper.ThrowIfNotOnUIThread();
            var uiShell = Package.GetGlobalService(typeof(SVsUIShell)) as IVsUIShell5;
            var color = uiShell.GetThemedWPFColor(EnvironmentColors.SystemWindowColorKey);
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            hostRootVisual.Background = brush;
        }

        private void SetVisualBrush(object sender, SizeChangedEventArgs e)
        {
            NativeMethods.GetWindowRect(((HwndSource)PresentationSource.FromVisual(Application.Current.MainWindow)).Handle,
                out RECT mainRect);
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
