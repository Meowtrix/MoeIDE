using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorBackground"/> class.
        /// </summary>
        /// <param name="view">The <see cref="IWpfTextView"/> upon which the adornment will be drawn</param>
        public EditorBackground(IWpfTextView view)
        {
            this.view = view;
            control = (ContentControl)view;
            view.Background = Brushes.Transparent;
            view.BackgroundBrushChanged += TextView_BackgroundBrushChanged;
            view.Closed += TextView_Closed;
            control.Loaded += TextView_Loaded;
        }

        private void TextView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (parentGrid == null) parentGrid = (Grid)control.Parent;
            if (viewStack == null) viewStack = (Canvas)control.Content;
            if (leftMargin == null) leftMargin = (Grid)(parentGrid.Children[1] as Panel).Children[0];
            MakeBackgroundTransparent();
        }

        private void TextView_Closed(object sender, EventArgs e)
        {
            view.Closed -= TextView_Closed;
            view.BackgroundBrushChanged -= TextView_BackgroundBrushChanged;
        }

        private void TextView_BackgroundBrushChanged(object sender, BackgroundBrushChangedEventArgs e)
        {
            control.Dispatcher.Invoke(() =>
            {
                while (parentGrid.Background != null) MakeBackgroundTransparent();
            }, DispatcherPriority.Render);
        }

        private void MakeBackgroundTransparent()
        {
            view.Background = Brushes.Transparent;
            viewStack.Background = Brushes.Transparent;
            leftMargin.Background = Brushes.Transparent;
            parentGrid.ClearValue(Panel.BackgroundProperty);
        }
    }
}
