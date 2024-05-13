using System.Numerics;

namespace DrawnUi.Maui.Draw;

public class SkiaSpringWithVelocityAnimator : SkiaVectorAnimator
{

    Vector2 Origin { get; set; }

    public void Initialize(Vector2 restOffset, Vector2 position, Vector2 velocity, Spring spring, float thresholdStop = 0.5f)
    {
        Origin = restOffset;

        Parameters = new SpringTimingParameters(spring, position, velocity, thresholdStop);
    }

    public SpringTimingParameters Parameters { get; set; }

    protected override bool UpdateValue(long deltaT, long deltaFromStart)
    {
        var secs = deltaFromStart / 1_000_000_000.0f;

        if (secs > Parameters.DurationSecs)
        {
            Vector = Origin;
            return true;
        }

        Vector = Origin + Parameters.ValueAt(secs);
        return false;
    }

    public SkiaSpringWithVelocityAnimator(IDrawnBase parent) : base(parent)
    {
    }
}