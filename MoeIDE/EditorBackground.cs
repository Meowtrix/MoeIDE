using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;
using Meowtrix.WPF.Extend;
using System.Linq;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorBackground"/> class.
        /// </summary>
        /// <param name="view">The <see cref="IWpfTextView"/> upon which the adornment will be drawn</param>
        public EditorBackground(IWpfTextView view)
        {
            this.view = view;
            control = (ContentControl)view;
            VSColorTheme.ThemeChanged += _ => control.Dispatcher.Invoke(MakeBackgroundTransparent, DispatcherPriority.Render);
            control.Loaded += TextView_Loaded;
        }

        private void TextView_Loaded(object sender, RoutedEventArgs e)
        {
            if (parentGrid == null) parentGrid = control.Parent as Grid;
            if (viewStack == null) viewStack = control.Content as Canvas;
            if (leftMargin == null) leftMargin = (parentGrid.BFS().FirstOrDefault(x => x.GetType().Name == "LeftMargin") as Grid)?.Children[0] as Grid;
            MakeBackgroundTransparent();
        }

        private void MakeBackgroundTransparent()
        {
            if (parentGrid != null) parentGrid.ClearValue(Panel.BackgroundProperty);
            if (viewStack != null) viewStack.Background = new VisualBrush(parentGrid);
            if (leftMargin != null) leftMargin.Background = Brushes.Transparent;
        }
    }
}
