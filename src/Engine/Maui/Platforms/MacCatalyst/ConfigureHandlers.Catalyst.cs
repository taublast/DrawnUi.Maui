using DrawnUi.Maui.Controls;

namespace DrawnUi.Maui.Draw
{
    public static partial class DrawnExtensions
    {
        public static void ConfigureHandlers(IMauiHandlersCollection handlers)
        {
            handlers.AddHandler(typeof(MauiEntry), typeof(MauiEntryHandler));
            handlers.AddHandler(typeof(MauiEditor), typeof(MauiEditorHandler));
            handlers.AddHandler(typeof(DrawnUiBasePage), typeof(DrawnUiBasePageHandler));

            //handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(SKMetalViewRenderer));
            //handlers.AddHandler(typeof(Window), typeof(CustomizedWindowHandler));

#if SKIA3
            handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(SKGLViewHandlerFixed));
#else
            handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(SKMetalViewRenderer));
#endif

        }
    }
}
