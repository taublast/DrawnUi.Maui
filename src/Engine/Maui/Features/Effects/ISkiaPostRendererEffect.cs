namespace DrawnUi.Maui.Draw;

public interface IPostRendererEffect : ISkiaEffect
{
    void Render(DrawingContext ctx);
}
