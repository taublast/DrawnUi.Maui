namespace DrawnUi.Maui.Draw;

public class BaseRenderEffect : SkiaEffect
{
    public virtual void Draw(SkiaControl parent, SKRect destination, SkiaDrawingContext ctx, Action<SkiaDrawingContext> drawControl)
    {

    }

}