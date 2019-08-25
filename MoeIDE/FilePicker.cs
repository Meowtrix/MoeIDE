using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.WindowsAPICodePack.Dialogs;

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

namespace Meowtrix.MoeIDE
{
    public enum FilePickerType { OpenFile, SaveFile, Folder }

    [TemplatePart(Name = nameof(PART_Button), Type = typeof(ButtonBase))]
    public class FilePicker : Control
    {
        static FilePicker() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FilePicker), new FrameworkPropertyMetadata(typeof(FilePicker)));

        private ButtonBase PART_Button;

        public string Filename
        {
            get => (string)GetValue(FilenameProperty);
            set => SetValue(FilenameProperty, value);
        }

        // Using a DependencyProperty as the backing store for Filename.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilenameProperty =
            DependencyProperty.Register(nameof(Filename), typeof(string), typeof(FilePicker), new PropertyMetadata(string.Empty));

        public event EventHandler<PropertyChangedEventArgs<string>> FilenameChanged;

        public FilePickerType PickerType
        {
            get => (FilePickerType)GetValue(PickerTypeProperty);
            set => SetValue(PickerTypeProperty, value);
        }

        // Using a DependencyProperty as the backing store for PickerType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PickerTypeProperty =
            DependencyProperty.Register(nameof(PickerType), typeof(FilePickerType), typeof(FilePicker), new PropertyMetadata(FilePickerType.OpenFile));

        public IEnumerable<string> Filters
        {
            get => (IEnumerable<string>)GetValue(FiltersProperty);
            set => SetValue(FiltersProperty, value);
        }

        // Using a DependencyProperty as the backing store for Filters.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FiltersProperty =
            DependencyProperty.Register(nameof(Filters), typeof(IEnumerable<string>), typeof(FilePicker), new PropertyMetadata(null));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (PART_Button != null)
                PART_Button.Click -= OnClick;
            PART_Button = GetTemplateChild(nameof(PART_Button)) as ButtonBase;
            if (PART_Button != null)
                PART_Button.Click += OnClick;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            CommonFileDialog dialog;
            if (PickerType == FilePickerType.OpenFile) dialog = new CommonOpenFileDialog();
            else if (PickerType == FilePickerType.SaveFile) dialog = new CommonSaveFileDialog();
            else dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            if (Filters != null)
                foreach (var f in Filters)
                    dialog.Filters.Add(new CommonFileDialogFilter(f, f));
            try
            {
                dialog.DefaultDirectory = Path.GetDirectoryName(Filename);
            }
            catch { }
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var old = Filename;
                Filename = dialog.FileName;
                FilenameChanged?.Invoke(this, new PropertyChangedEventArgs<string>(old, Filename));
            }
            dialog.Dispose();
        }
    }
}
