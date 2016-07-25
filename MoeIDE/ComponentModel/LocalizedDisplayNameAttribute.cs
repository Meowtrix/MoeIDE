using System.ComponentModel;
using System.Resources;

namespace Meowtrix.MoeIDE.ComponentModel
{
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        public static ResourceManager resources;
        private readonly string _key;
        public LocalizedDisplayNameAttribute(string key) { _key = key; }
        public override string DisplayName
            => resources?.GetString(_key) ?? base.DisplayName;
    }
}
