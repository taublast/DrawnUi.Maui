namespace DrawnUi.Maui.Draw;

public class RippleAnimator : RenderingAnimator
{
    public RippleAnimator(IDrawnBase control) : base(control)
    {
        IsPostAnimator = true;
        Speed = 500;//250;
        mMinValue = 0;
        mMaxValue = 1;
        Color = SKColor.Parse("#FFFFFF");
        Easing = Easing.CubicIn;
    }

    protected static long count;

    public SKColor Color { get; set; }

    public static double DiameterDefault = 300.0;

    public static double OpacityDefault = 0.20;

    /// <summary>
    /// In pts relative to control X,Y. These are coords inside the control and not inside the canvas. 
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// In pts relative to control X,Y. These are coords inside the control and not inside the canvas. 
    /// </summary>
    public double Y { get; set; }

    public double Diameter { get; set; }

    public double Opacity { get; set; }

    public override void Dispose()
    {
        if (Parent != null)
        {
            Parent.DisposeObject(Paint);
        }

        base.Dispose();
    }

    protected SKPaint Paint;

    protected override bool OnRendering(IDrawnBase control, SkiaDrawingContext context, double scale)
    {
        if (IsRunning && control != null && !control.IsDisposed && !control.IsDisposing)
        {
            var touchOffset = new SKPoint((float)(X * scale), (float)(Y * scale));
            var selfDrawingLocation = GetSelfDrawingLocation(control);

            DrawWithClipping(context, control, selfDrawingLocation, () =>
            {
                Paint ??= new SKPaint();
                Paint.Style = SKPaintStyle.Fill;
                Paint.Color = Color.WithAlpha((byte)(Opacity * 255));

                touchOffset.Offset(selfDrawingLocation);
                context.Canvas.DrawCircle(touchOffset.X, touchOffset.Y, (float)(Diameter * scale), Paint);
            });

            return true;
        }

        return false;
    }

    protected override double TransformReportedValue(long deltaT)
    {

        var progress = base.TransformReportedValue(deltaT);

        var opacityProgress = progress * 1.15;
        if (opacityProgress <= 1)
        {
            Opacity = OpacityDefault - OpacityDefault * opacityProgress;
        }

        Diameter = DiameterDefault * progress;

        return progress;
    }

}
