namespace DrawnUi.Maui.Draw;

public class PresetEffect : SkiaEffect
{
    public static readonly BindableProperty PresetProperty = BindableProperty.Create(
        nameof(Preset),
        typeof(SkiaImageEffect),
        typeof(SkiaImage),
        SkiaImageEffect.Pastel,
        propertyChanged: NeedUpdate);

    public SkiaImageEffect Preset
    {
        get { return (SkiaImageEffect)GetValue(PresetProperty); }
        set { SetValue(PresetProperty, value); }
    }

    public override void Attach(SkiaControl parent)
    {
        base.Attach(parent);

        parent.CustomizeLayerPaint = (paint, rect) =>
        {
            if (paint != null)
            {
                paint.ColorFilter = Preset switch
                {
                    SkiaImageEffect.BlackAndWhite
                        => SkiaImageEffects.Grayscale(),

                    SkiaImageEffect.Pastel
                        => SkiaImageEffects.Pastel(),

                    SkiaImageEffect.Sepia
                        => SkiaImageEffects.Sepia(),

                    SkiaImageEffect.InvertColors
                        => SkiaImageEffects.InvertColors(),

                    _ => null
                };
            }
        };
    }
}