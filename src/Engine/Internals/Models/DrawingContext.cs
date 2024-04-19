namespace DrawnUi.Maui.Draw;

public class SkiaDrawingContext
{
    public SKCanvas Canvas { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public long FrameTimeNanos { get; set; }
    public DrawnView Superview { get; set; }
    public string Tag { get; set; }

    /// <summary>
    /// Recording cache
    /// </summary>
    public bool IsVirtual { get; set; }

    /// <summary>
    /// Reusing surface from previous cache
    /// </summary>
    public bool IsRecycled { get; set; }

    public SkiaDrawingContext Clone()
    {
        return new SkiaDrawingContext()
        {
            Superview = Superview,
            Width = Width,
            Height = Height,
            Canvas = this.Canvas,
            FrameTimeNanos = this.FrameTimeNanos,
        };
    }

    public static float DeviceDensity
    {
        get
        {
            return (float)Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
        }
    }

    public static IServiceProvider Services
        =>
#if WINDOWS10_0_19041_0_OR_GREATER
            MauiWinUIApplication.Current.Services;
#elif ANDROID
            MauiApplication.Current.Services;
#elif IOS || MACCATALYST
            MauiUIApplicationDelegate.Current.Services;
#else
            null;


#endif


}