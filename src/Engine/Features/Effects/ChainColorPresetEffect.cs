namespace DrawnUi.Maui.Draw;

public class ChainColorPresetEffect : BaseChainedEffect
{
    #region PROPERTIES

    public static readonly BindableProperty PresetProperty = BindableProperty.Create(
        nameof(Preset),
        typeof(SkiaImageEffect),
        typeof(ChainColorPresetEffect),
        SkiaImageEffect.Pastel,
        propertyChanged: NeedUpdate);

    public SkiaImageEffect Preset
    {
        get { return (SkiaImageEffect)GetValue(PresetProperty); }
        set { SetValue(PresetProperty, value); }
    }

    #endregion

    public override ChainEffectResult Draw(SKRect destination, SkiaDrawingContext ctx, Action<SkiaDrawingContext> drawControl)
    {
        if (NeedApply)
        {
            if (Paint == null)
            {
                Paint = new()
                {
                    ColorFilter = Preset switch
                    {
                        SkiaImageEffect.Grayscale
                            => SkiaImageEffects.Grayscale2(),

                        SkiaImageEffect.BlackAndWhite
                            => SkiaImageEffects.Grayscale(),

                        SkiaImageEffect.Pastel
                            => SkiaImageEffects.Pastel(),

                        SkiaImageEffect.Sepia
                            => SkiaImageEffects.Sepia(),

                        SkiaImageEffect.InvertColors
                            => SkiaImageEffects.InvertColors(),

                        _ => null
                    }
                };
            }

            var restore = ctx.Canvas.SaveLayer(Paint);

            drawControl(ctx);

            return ChainEffectResult.Create(true, restore);
        }

        return base.Draw(destination, ctx, drawControl);
    }

    public override bool NeedApply
    {
        get
        {
            return base.NeedApply && (this.Preset != SkiaImageEffect.None);
        }
    }
}