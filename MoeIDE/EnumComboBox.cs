using System;
using System.Linq;
using System.Windows.Controls;

namespace Meowtrix.MoeIDE
{
    public class EnumComboBox : ComboBox
    {
        private Type _enumtype;
        public Type EnumType
        {
            get { return _enumtype; }
            set
            {
                if (!value.IsEnum) throw new ArgumentException(nameof(EnumType));
                _enumtype = value;
                BuildItemsSource(value);
            }
        }

        private class EnumComboBoxItem
        {
            public string Name { get; }
            public Enum Value { get; }
            public EnumComboBoxItem(Enum @enum)
            {
                Value = @enum;
                Name = @enum.ToString();
            }
        }

        private void BuildItemsSource(Type type)
        {
            ItemsSource = Enum.GetValues(type).Cast<Enum>().Select(x => new EnumComboBoxItem(x)).ToArray();
            SelectedValuePath = nameof(EnumComboBoxItem.Value);
            DisplayMemberPath = nameof(EnumComboBoxItem.Name);
            SelectedIndex = 0;
        }
    }
}
