using System;
using System.Resources;
using System.Windows.Markup;

namespace Meowtrix.MoeIDE
{
    internal class LocalizedExtension : MarkupExtension
    {
        public static ResourceManager resources;
        [ConstructorArgument("resourceKey")]
        public string ResourceKey { get; set; }
        public LocalizedExtension() { }
        public LocalizedExtension(string resourceKey)
        {
            ResourceKey = resourceKey;
        }
        public override object ProvideValue(IServiceProvider serviceProvider) => resources?.GetString(ResourceKey) ?? ResourceKey;
    }
}
