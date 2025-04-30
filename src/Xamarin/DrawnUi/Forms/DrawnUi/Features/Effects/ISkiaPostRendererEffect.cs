namespace DrawnUi.Draw;

public interface IPostRendererEffect : ISkiaEffect
{
    void Render(SkiaDrawingContext ctx, SKRect destination);
}