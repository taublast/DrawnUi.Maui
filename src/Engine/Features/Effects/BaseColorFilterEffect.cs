namespace DrawnUi.Maui.Draw;

public class BaseColorFilterEffect : SkiaEffect
{
    public SKColorFilter Filter { get; set; }

    public virtual SKColorFilter CreateFilter(SkiaControl parent, SKRect destination)
    {
        return null;
    }


    protected override void OnDisposing()
    {
        Filter?.Dispose();

        base.OnDisposing();
    }
}