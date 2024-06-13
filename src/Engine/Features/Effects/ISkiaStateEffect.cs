namespace DrawnUi.Maui.Draw;

public interface ISkiaStateEffect : ISkiaEffect
{
    /// <summary>
    /// Will be invoked before actually painting but after gestures processing and other internal calculations
    /// </summary>
    void UpdateState();
}