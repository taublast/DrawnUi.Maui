namespace DrawnUi.Maui.Draw;

public class BlurEffect : SkiaEffect
{
    public static readonly BindableProperty AmountProperty = BindableProperty.Create(
        nameof(Amount),
        typeof(double),
        typeof(SkiaImage),
        0.3,
        propertyChanged: NeedUpdate);

    public double Amount
    {
        get { return (double)GetValue(AmountProperty); }
        set { SetValue(AmountProperty, value); }
    }

    public override void Attach(SkiaControl parent)
    {
        base.Attach(parent);

        parent.CustomizeLayerPaint = (paint, rect) =>
        {
            if (paint != null)
            {
                if (Amount > 0)
                {
                    paint.ImageFilter = SKImageFilter.CreateBlur((float)Amount, (float)Amount, SKShaderTileMode.Mirror);
                }
                else
                {
                    paint.ImageFilter = null;

                }
            }
        };
    }
}