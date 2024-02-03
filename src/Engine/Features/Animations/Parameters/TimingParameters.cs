using System.Numerics;

namespace DrawnUi.Maui.Draw;

public interface ITimingParameters
{
    float DurationSecs { get; }
    Vector2 ValueAt(float offsetSecs);
}