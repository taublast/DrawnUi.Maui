#if DEBUG
using Sandbox;

[assembly: System.Reflection.Metadata.MetadataUpdateHandlerAttribute(typeof(HotReloadService))]

namespace Sandbox
{
    public static class HotReloadService
    {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public static event Action<Type[]?>? UpdateApplicationEvent;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        internal static void ClearCache(Type[]? types) { }
        internal static void UpdateApplication(Type[]? types)
        {
            UpdateApplicationEvent?.Invoke(types); //todo attach Super
        }
    }
}
#endif


