using System.ComponentModel;
using System.Resources;

namespace Meowtrix.MoeIDE.ComponentModel
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public static ResourceManager resources;
        public readonly string _key;
        public LocalizedDescriptionAttribute(string key) { _key = key; }
        public override string Description
            => resources.GetString(_key) ?? base.Description;
    }
}
