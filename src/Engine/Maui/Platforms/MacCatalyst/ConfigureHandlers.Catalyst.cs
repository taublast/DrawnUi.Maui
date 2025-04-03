using DrawnUi.Controls;

namespace DrawnUi.Draw
{
    public static partial class DrawnExtensions
    {
        public static void ConfigureHandlers(IMauiHandlersCollection handlers)
        {
            handlers.AddHandler(typeof(MauiEntry), typeof(MauiEntryHandler));
            handlers.AddHandler(typeof(MauiEditor), typeof(MauiEditorHandler));
            handlers.AddHandler(typeof(DrawnUiBasePage), typeof(DrawnUiBasePageHandler));

#if SKIA3
            handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(SKGLViewHandlerRetained));
#else
            handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(SKMetalViewRenderer));
#endif

        }
    }
}
