namespace DrawnUi.Maui.Draw;

public class BaseImageFilterEffect : SkiaEffect
{
    public SKImageFilter Filter { get; set; }

    public virtual SKImageFilter CreateFilter(SkiaControl parent, SKRect destination)
    {
        return null;
    }

    protected override void OnDisposing()
    {
        Filter?.Dispose();

        base.OnDisposing();
    }
}