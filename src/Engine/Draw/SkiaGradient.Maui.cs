namespace DrawnUi.Maui.Draw;

public partial class SkiaGradient : BindableObject, ICloneable
{
    #region MAUI

    public static SkiaGradient FromBrush(GradientBrush gradientBrush)
    {
        SkiaGradient gradient = null;

        if (gradientBrush is LinearGradientBrush linear)
        {
            gradient = new SkiaGradient()
            {
                Type = GradientType.Linear,
                Colors = linear.GradientStops.Select(x => x.Color).ToList(),
                ColorPositions = linear.GradientStops.Select(x => (double)x.Offset).ToList(),
                StartXRatio = (float)linear.StartPoint.X,
                StartYRatio = (float)linear.StartPoint.Y,
                EndXRatio = (float)linear.EndPoint.X,
                EndYRatio = (float)linear.EndPoint.Y,
            };
        }
        else
        if (gradientBrush is RadialGradientBrush radial)
        {
            gradient = new SkiaGradient()
            {
                Type = GradientType.Oval, //MAUI is using oval and not circle for this
                Colors = radial.GradientStops.Select(x => x.Color).ToList(),
                ColorPositions = radial.GradientStops.Select(x => (double)x.Offset).ToList(),
                StartXRatio = (float)radial.Center.X,
                StartYRatio = (float)radial.Center.Y
            };
        }
        return gradient;
    }
    #endregion
}