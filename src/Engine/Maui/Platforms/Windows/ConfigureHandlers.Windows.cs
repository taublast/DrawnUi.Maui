using DrawnUi.Controls;

namespace DrawnUi.Draw
{
    public static partial class DrawnExtensions
    {
        public static void ConfigureHandlers(IMauiHandlersCollection handlers)
        {
            handlers.AddHandler(typeof(SkiaView), typeof(WinCanvasHandler));
            handlers.AddHandler(typeof(MauiEntry), typeof(MauiEntryHandler));
            handlers.AddHandler(typeof(MauiEditor), typeof(MauiEditorHandler));

#if SKIA3
            //handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(SKGLViewHandlerRetained));
            handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(SKGLViewHandlerSafe));
#endif
        }
    }
}
