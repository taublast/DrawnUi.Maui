
[assembly: System.Reflection.Metadata.MetadataUpdateHandlerAttribute(typeof(DrawnUi.HotReloadService))]
namespace DrawnUi
{
    public static class HotReloadService
    {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public static event Action<Type[]?>? UpdateApplicationEvent;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        internal static void ClearCache(Type[]? types) { }

        internal static void UpdateApplication(Type[]? types)
        {
            if (Debugger.IsAttached)
            {
                UpdateApplicationEvent?.Invoke(types);
            }
        }

    }
}


