namespace DrawnUi.Maui.Draw;

public interface IDampingTimingParameters : ITimingParameters
{
    float AmplitudeAt(float offsetSecs);
}