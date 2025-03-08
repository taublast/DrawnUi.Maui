using System.Numerics;

namespace DrawnUi.Maui.Draw;

public interface ITimingVectorParameters
{
    float DurationSecs { get; }
    Vector2 ValueAt(float offsetSecs);
}