using DrawnUi.Controls;

namespace DrawnUi.Draw
{
    public static partial class DrawnExtensions
    {
        public static void ConfigureHandlers(IMauiHandlersCollection handlers)
        {
            handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(SKGLViewHandlerRetained));
            handlers.AddHandler(typeof(MauiEntry), typeof(MauiEntryHandler));
            handlers.AddHandler(typeof(MauiEditor), typeof(MauiEditorHandler));
            handlers.AddHandler(typeof(DrawnUiBasePage), typeof(DrawnUiBasePageHandler));
        }
    }
}
