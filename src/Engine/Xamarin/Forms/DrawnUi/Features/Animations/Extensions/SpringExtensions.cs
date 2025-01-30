using DrawnUi.Maui.Infrastructure;

namespace DrawnUi.Maui.Draw;

public static class SpringExtensions
{
    public static float Damping(this Spring spring)
    {
        return (float)(2 * spring.DampingRatio * Math.Sqrt(spring.Mass * spring.Stiffness));
    }

    public static float Beta(this Spring spring)
    {
        return spring.Damping() / (2 * spring.Mass);
    }

    public static float DampedNaturalFrequency(this Spring spring)
    {
        return (float)(Math.Sqrt(spring.Stiffness / spring.Mass) * Math.Sqrt(1 - spring.DampingRatio * spring.DampingRatio));
    }
}