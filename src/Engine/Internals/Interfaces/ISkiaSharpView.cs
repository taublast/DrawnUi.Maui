namespace DrawnUi.Maui.Draw;

public interface ISkiaSharpView
{
    /// <summary>
    /// Safe InvalidateSurface() call
    /// </summary>
    public void Update();

    public void PostponeInvalidation();

    public SKSize CanvasSize { get; }

    /// <summary>
    /// This is needed to get a hardware accelerated surface or a normal one depending on the platform
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public SKSurface CreateStandaloneSurface(int width, int height);

}