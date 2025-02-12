namespace DrawnUi.Maui.Draw;

public class BaseChainedEffect : SkiaEffect, IRenderEffect
{
    public SKPaint Paint { get; set; }

    public virtual ChainEffectResult Draw(DrawingContext ctx, Action<DrawingContext> drawControl)
    {
        return ChainEffectResult.Default;
    }

    public override void Update()
    {
        var kill = Paint;
        Paint = null;
        kill?.Dispose();

        base.Update();
    }

    protected override void OnDisposing()
    {
        Paint?.Dispose();

        base.OnDisposing();
    }
}

