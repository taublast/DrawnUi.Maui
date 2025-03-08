namespace DrawnUi.Maui.Infrastructure;

public struct SkiaTouchResultContext
{
    public object Context { get; set; }
    public SkiaControl Control { get; set; }
    public TouchActionEventArgs TouchArgs { get; set; }
    public TouchActionResult TouchAction { get; set; }
}