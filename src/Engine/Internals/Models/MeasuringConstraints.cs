namespace DrawnUi.Maui.Infrastructure;

public struct MeasuringConstraints
{
    public Thickness Margins { get; set; }
    public SKSize Request { get; set; }
    public SKRect Content { get; set; }
}