namespace DrawnUi.Maui.Draw;

/// <summary>
/// Paints the parent view with the background color with a clipped viewport oth this view size
/// </summary>
public class SkiaHoverMask : SkiaShape
{
    protected override void Paint(SkiaDrawingContext ctx, SKRect destination, float scale, object arguments)
    {

        //paint clipped background
        if (BackgroundColor != Colors.Transparent && Parent != null)
        {
            using (var paint = new SKPaint
            {
                Color = BackgroundColor.ToSKColor(),
                Style = SKPaintStyle.StrokeAndFill,
            })
            {
                SetupGradient(paint, FillGradient, destination);

                using (SKPath clipInsideParent = new SKPath())
                {
                    ctx.Canvas.Save();

                    using var clipContent = CreateClip(arguments, true);
                    clipInsideParent.AddPath(clipContent);

                    ctx.Canvas.ClipPath(clipInsideParent, SKClipOperation.Difference, true);

                    //paint this taking viewport dimensions
                    ctx.Canvas.DrawRect(Parent.DrawingRect, paint);

                    DrawViews(ctx, destination, scale);

                    //todo add stroke property?

                    ctx.Canvas.Restore();
                }
            }

        }

    }
}