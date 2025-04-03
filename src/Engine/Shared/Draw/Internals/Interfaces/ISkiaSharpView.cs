namespace DrawnUi.Draw;

public interface ISkiaSharpView
{
    /// <summary>
    /// Safe InvalidateSurface() call. If nanos not specified will generate ittself
    /// </summary>
    public void Update(long nanos = 0);

    public void SignalFrame(long nanoseconds);

    public SKSize CanvasSize { get; }

    /// <summary>
    /// This is needed to get a hardware accelerated surface or a normal one depending on the platform
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public SKSurface CreateStandaloneSurface(int width, int height);

}