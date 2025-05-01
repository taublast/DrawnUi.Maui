// Credits to https://github.com/super-ultra

using System.Runtime.CompilerServices;

namespace DrawnUi.Draw;

public class ScrollFlingAnimator : SkiaValueAnimator
{
    public bool SelfFinished { get; set; }

    public Task RunAsync(float position, float velocity, float deceleration = 0.998f, float threshold = 0.5f, CancellationToken cancellationToken = default)
    {
        return RunAsync(() => InitializeWithVelocity(position, velocity, deceleration, threshold), cancellationToken);
    }

    public void InitializeWithVelocity(float position, float velocity, float deceleration = 0.998f, float threshold = 0.5f)
    {
        Parameters = new(position, velocity, deceleration, threshold);
        Speed = Parameters.DurationSecs;
    }

    public void InitializeWithDestination(float position, float target, float timeSecs, float deceleration = 0.998f, float threshold = 0.5f)
    {
        Parameters = new(position, target, timeSecs, deceleration, threshold);
        Speed = Parameters.DurationSecs;
    }

    public DecelerationTimingParameters Parameters { get; set; }

    public float CurrentVelocity { get; protected set; }

    public override void Start(double delayMs = 0)
    {
        SelfFinished = false;

        base.Start(delayMs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override bool UpdateValue(long deltaT, long deltaFromStart)
    {
        var secs = deltaFromStart / 1_000_000_000.0f;

        if (secs > Speed)
        {
            SelfFinished = true;
        }

        mValue = Parameters.ValueAt(secs);
        CurrentVelocity = Parameters.VelocityAt(secs);
        return SelfFinished;
    }

    public ScrollFlingAnimator(IDrawnBase parent) : base(parent)
    {
        InitializeWithVelocity(0, 0);
    }

}
