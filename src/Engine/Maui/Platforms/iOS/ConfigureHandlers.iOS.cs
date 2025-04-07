using DrawnUi.Controls;
using Microsoft.Maui.Handlers;

namespace DrawnUi.Draw
{
    public static partial class DrawnExtensions
    {
        public static void ConfigureHandlers(IMauiHandlersCollection handlers)
        {
            bool useFullScreen = false;
            if (StartupSettings != null && StartupSettings.MobileIsFullscreen)
            {
                useFullScreen = StartupSettings.MobileIsFullscreen;
            }
            var useSafeArea = !useFullScreen;

            PageHandler.Mapper.AppendToMapping("Custom", (h, v) =>
                 {
                     if (v is Microsoft.Maui.Controls.Page page)
                     {
                         Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(page, useSafeArea);
                     }
                 });

            LayoutHandler.Mapper.AppendToMapping("Custom", (h, v) =>
            {
                if (v is Layout layout)
                {
                    layout.IgnoreSafeArea = useFullScreen;
                }
            });

            handlers.AddHandler(typeof(MauiEntry), typeof(MauiEntryHandler));
            handlers.AddHandler(typeof(MauiEditor), typeof(MauiEditorHandler));

#if SKIA3
            handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(SKGLViewHandlerRetained));
#else
            handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(SKMetalViewRenderer));
#endif
        }
    }

    namespace DrawnUi.Views
    {
    }

}
