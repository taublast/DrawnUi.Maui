namespace DrawnUi.Draw;

public interface IDrawnTextSpan
{
    public void Render(SkiaDrawingContext ctx, SKRect destination, float scale);

    public ScaledSize Measure(float maxWidth, float maxHeight, float scale);

    public DrawImageAlignment VerticalAlignement { get; }
}