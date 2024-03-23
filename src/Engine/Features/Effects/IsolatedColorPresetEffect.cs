namespace DrawnUi.Maui.Draw;

public class IsolatedColorPresetEffect : BaseRenderEffect
{
    #region PROPERTIES

    public static readonly BindableProperty PresetProperty = BindableProperty.Create(
        nameof(Preset),
        typeof(SkiaImageEffect),
        typeof(IsolatedColorPresetEffect),
        SkiaImageEffect.Pastel,
        propertyChanged: NeedUpdate);

    public SkiaImageEffect Preset
    {
        get { return (SkiaImageEffect)GetValue(PresetProperty); }
        set { SetValue(PresetProperty, value); }
    }

    #endregion

    public override void Draw(SkiaControl parent, SKRect destination, SkiaDrawingContext ctx, Action<SkiaDrawingContext> drawControl)
    {
        if (NeedApply)
        {
            using var paint = new SKPaint();

            paint.ColorFilter = Preset switch
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
            };

            var restore = ctx.Canvas.SaveLayer(paint);

            drawControl(ctx);

            if (restore != 0)
                ctx.Canvas.RestoreToCount(restore);
        }
    }

    public override bool NeedApply
    {
        get
        {
            return base.NeedApply && (this.Preset != SkiaImageEffect.None);
        }
    }
}