namespace DrawnUi.Draw;

public class BaseColorFilterEffect : SkiaEffect, IColorEffect
{
    public SKColorFilter Filter { get; set; }

    public virtual SKColorFilter CreateFilter(SKRect destination)
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

        base.OnDisposing();
    }
}