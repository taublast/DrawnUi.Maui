using System.Numerics;

namespace DrawnUi.Maui.Draw;

public interface IDampingTimingVectorParameters : ITimingVectorParameters
{
    Vector2 AmplitudeAt(float offsetSecs);
}