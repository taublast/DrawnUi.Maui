namespace DrawnUi.Maui.Draw;

public class BaseImageFilterEffect : SkiaEffect, IImageEffect
{
    public SKImageFilter Filter { get; set; }

    public virtual SKImageFilter CreateFilter(SKRect destination)
    {
        return null;
    }

    public override void Update()
    {
        var kill = Filter;
        Filter = null;
        kill?.Dispose();
        base.Update();
    }

    protected override void OnDisposing()
    {
        Filter?.Dispose();
        Filter = null;

        base.OnDisposing();
    }

}