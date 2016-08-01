using System;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Meowtrix.MoeIDE
{
    [Serializable, XmlRoot("Settings")]
    public class SettingsModel
    {
        [Serializable]
        public class ImageInfo
        {
            public string Filename { get; set; }
            public Stretch Stretch { get; set; } = Stretch.UniformToFill;
            public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;
            public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;
            public Color BackColor { get; set; } = Colors.Transparent;
            public double Opacity { get; set; } = 1.0;
            public double Blur { get; set; } = 0.0;
            public ImageInfo Clone() => (ImageInfo)MemberwiseClone();
        }
        public ImageInfo MainBackground { get; set; } = new ImageInfo();
        public SettingsModel Clone() => new SettingsModel
        {
            MainBackground = this.MainBackground.Clone()
        };
    }
}
