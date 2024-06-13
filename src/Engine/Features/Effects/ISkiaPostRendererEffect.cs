namespace DrawnUi.Maui.Draw;

public interface IPostRendererEffect : ISkiaEffect
{
    void Render(SkiaDrawingContext ctx, SKRect destination);
}