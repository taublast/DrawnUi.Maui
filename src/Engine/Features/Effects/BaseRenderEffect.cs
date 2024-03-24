namespace DrawnUi.Maui.Draw;

public class BaseRenderEffect : SkiaEffect, IRenderEffect
{
    public virtual bool Draw(SKRect destination, SkiaDrawingContext ctx, Action<SkiaDrawingContext> drawControl)
    {
        return false;
    }

}