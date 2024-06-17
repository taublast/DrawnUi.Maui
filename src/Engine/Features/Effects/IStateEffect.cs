namespace DrawnUi.Maui.Draw;

public interface IStateEffect : ISkiaEffect
{
    /// <summary>
    /// Will be invoked before actually painting but after gestures processing and other internal calculations
    /// </summary>
    void UpdateState();
}