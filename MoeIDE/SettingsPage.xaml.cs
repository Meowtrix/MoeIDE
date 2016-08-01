using System.Windows;
using System.Windows.Controls;

namespace Meowtrix.MoeIDE
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void DefaultColor(object sender, RoutedEventArgs e)
        {
            var exp = textColor.GetBindingExpression(TextBox.TextProperty);
            textColor.Text = (sender as Control).Tag.ToString();
            exp.UpdateSource();
        }
    }
}
