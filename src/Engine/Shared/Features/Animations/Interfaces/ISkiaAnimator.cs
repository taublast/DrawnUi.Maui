namespace DrawnUi.Maui.Draw;

public interface ISkiaAnimator : IDisposable
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="frameTime"></param>
    /// <returns>Is Finished</returns>
    bool TickFrame(long frameTime);

    bool IsRunning { get; }

    bool WasStarted { get; }

    Guid Uid { get; }

    void Stop();

    /// <summary>
    /// Used by ui, please use play stop for manual control
    /// </summary>
    void Pause();

    /// <summary>
    /// Used by ui, please use play stop for manual control
    /// </summary>
    void Resume();

    void Start(double delayMs = 0);

    public IDrawnBase Parent { get; }

    /// <summary>
    /// Can and will be removed
    /// </summary>
    bool IsDeactivated { get; set; }

    /// <summary>
    /// Just should not execute on tick
    /// </summary>
    bool IsPaused { get; set; }

    /// <summary>
    /// For internal use by the engine
    /// </summary>
    bool IsHiddenInViewTree { get; set; }

}