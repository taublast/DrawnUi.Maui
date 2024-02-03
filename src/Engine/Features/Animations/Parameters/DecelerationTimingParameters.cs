// Credits to https://github.com/super-ultra

using System.Numerics;
using System.Runtime.CompilerServices;

namespace DrawnUi.Maui.Draw;


public class DecelerationTimingParameters : ITimingParameters
{

    public Vector2 InitialValue { get; set; }
    public Vector2 InitialVelocity { get; set; }
    public float DecelerationRate { get; set; }
    public float Threshold { get; set; }

    public DecelerationTimingParameters(Vector2 initialValue, Vector2 initialVelocity, float decelerationRate, float threshold)
    {

        if (decelerationRate <= 0 || decelerationRate >= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(decelerationRate), "Deceleration rate must be greater than 0 and less than 1.");
        }

        InitialValue = initialValue;
        InitialVelocity = initialVelocity;
        DecelerationRate = decelerationRate;
        Threshold = threshold;
    }

    public Vector2 Destination
    {
        get
        {
            float dCoeff = 1000 * (float)Math.Log(DecelerationRate);
            return InitialValue - InitialVelocity / dCoeff;
        }
    }

    public float DurationSecs
    {
        get
        {
            if (InitialVelocity.Length() == 0)
                return 0;

            float dCoeff = 1000 * (float)Math.Log(DecelerationRate);
            return (float)Math.Log(-dCoeff * Threshold / InitialVelocity.Length()) / dCoeff;
        }
    }

    /// <summary>
    /// time is in seconds
    /// </summary>
    /// <param name="offsetSecs"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 ValueAt(float offsetSecs)
    {
        float time = offsetSecs;

        float dCoeff = 1000 * (float)Math.Log(DecelerationRate);
        float factor = (float)((Math.Pow(DecelerationRate, (float)(1000 * time)) - 1) / dCoeff);
        return InitialValue + InitialVelocity * factor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 VelocityAt(double time)
    {
        return InitialVelocity * (float)Math.Pow(DecelerationRate, (float)(1000 * time));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double? DurationToValue(Vector2 value)
    {
        if (DistanceToSegment(value, InitialValue, Destination) >= Threshold)
            return null;

        float dCoeff = 1000 * (float)Math.Log(DecelerationRate);
        return Math.Log(1 + dCoeff * (value - InitialValue).Length() / InitialVelocity.Length()) / dCoeff;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 VelocityTo(Vector2 startingPoint, Vector2 targetPoint, double time)
    {
        float dCoeff = 1000 * (float)Math.Log(DecelerationRate);
        float factor = (float)(Math.Pow(DecelerationRate, 1000 * time) - 1);

        return (targetPoint - startingPoint) * dCoeff / factor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 VelocityToZero(Vector2 startingPoint, Vector2 targetPoint, float maxTimeSecs = 0, float epsilon = 1e-6f)
    {
        Vector2 distance = targetPoint - startingPoint;
        float distanceMagnitude = distance.Length();

        // Calculate the time at which the velocity will be epsilon
        float optimalTime = (float)(Math.Log(epsilon) - Math.Log(distanceMagnitude)) / (1000 * (float)Math.Log(DecelerationRate));

        if (maxTimeSecs > 0 && optimalTime > maxTimeSecs)
            optimalTime = maxTimeSecs;

        // Calculate the initial velocity needed to reach the target point in optimalTime
        Vector2 initialVelocity = VelocityTo(startingPoint, targetPoint, optimalTime);

        return initialVelocity;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DistanceToSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
    {
        Vector2 segment = segmentEnd - segmentStart;
        Vector2 v = point - segmentStart;
        float t = Vector2.Dot(v, segment) / Vector2.Dot(segment, segment);

        if (t <= 0)
            return Vector2.Distance(point, segmentStart);
        if (t >= 1)
            return Vector2.Distance(point, segmentEnd);

        Vector2 projection = segmentStart + t * segment;
        return Vector2.Distance(point, projection);
    }
}