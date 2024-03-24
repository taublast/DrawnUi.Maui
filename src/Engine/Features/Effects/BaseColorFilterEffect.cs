namespace DrawnUi.Maui.Draw;

public class BaseColorFilterEffect : SkiaEffect, IColorEffect
{
    public SKColorFilter Filter { get; set; }

    public virtual SKColorFilter CreateFilter(SKRect destination)
    {
        return null;
    }

    protected override void OnDisposing()
    {
        Filter?.Dispose();

        base.OnDisposing();
    }
}