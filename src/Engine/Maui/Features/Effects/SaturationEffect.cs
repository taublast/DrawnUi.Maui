namespace DrawnUi.Draw;

public class SaturationEffect : BaseColorFilterEffect
{
    public static readonly BindableProperty SaturationProperty = BindableProperty.Create(
        nameof(Saturation),
        typeof(float),
        typeof(SkiaImage),
        1f, // Default to no change
        propertyChanged: NeedUpdate);

    public float Saturation
    {
        get => (float)GetValue(SaturationProperty);
        set => SetValue(SaturationProperty, value);
    }

    public override SKColorFilter CreateFilter(SKRect destination)
    {
        if (NeedApply)
        {
            if (Filter == null)
            {
                Filter = CreateSaturationFilter(Saturation);
            }
        }
        return Filter;
    }

    private SKColorFilter CreateSaturationFilter(float saturation)
    {
        float invSat = 1 - saturation;
        float R = 0.213f * invSat;
        float G = 0.715f * invSat;
        float B = 0.072f * invSat;

        float[] colorMatrix = {
            R + saturation, G, B, 0, 0,
            R, G + saturation, B, 0, 0,
            R, G, B + saturation, 0, 0,
            0, 0, 0, 1, 0
        };

        return SKColorFilter.CreateColorMatrix(colorMatrix);
    }

    public override bool NeedApply => base.NeedApply && Saturation != 1f;
}