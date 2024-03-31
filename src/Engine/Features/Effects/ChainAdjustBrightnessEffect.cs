namespace DrawnUi.Maui.Draw;

public class ChainAdjustBrightnessEffect : BaseChainedEffect
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(float),
        typeof(ChainAdjustBrightnessEffect),
        1f,
        propertyChanged: NeedUpdate);

    public float Value
    {
        get => (float)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public override ChainEffectResult Draw(SKRect destination, SkiaDrawingContext ctx, Action<SkiaDrawingContext> drawControl)
    {
        if (NeedApply)
        {
            if (Paint == null)
            {
                Paint = new()
                {
                    ColorFilter = CreateBrightnessFilter(Value)
                };
            }

            var restore = ctx.Canvas.SaveLayer(Paint);

            drawControl(ctx);

            return ChainEffectResult.Create(true, restore);
        }

        return base.Draw(destination, ctx, drawControl);
    }

    private SKColorFilter CreateBrightnessFilter(float value)
    {
        var brightness = Math.Clamp(value - 1, -1f, 1f);

        float b = brightness; // Assuming brightness is in [0, 1] range; adjust if it's [-1, 1]
        float[] colorMatrix = {
            1, 0, 0, 0, b,
            0, 1, 0, 0, b,
            0, 0, 1, 0, b,
            0, 0, 0, 1, 0
        };

        return SKColorFilter.CreateColorMatrix(colorMatrix);
    }

    public override bool NeedApply => base.NeedApply && Value != 1f;
}