using System.Numerics;

namespace DrawnUi.Maui.Draw;

public interface IDampingTimingParameters : ITimingParameters
{
    Vector2 AmplitudeAt(float offsetSecs);
}