using System.ComponentModel;
using System.Resources;

namespace Meowtrix.MoeIDE.ComponentModel
{
    public class LocalizedCategoryAttribute : CategoryAttribute
    {
        public static ResourceManager resources;
        public LocalizedCategoryAttribute(string name) : base(name) { }
        protected override string GetLocalizedString(string value)
            => resources?.GetString(value) ?? base.GetLocalizedString(value);
    }
}
