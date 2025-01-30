namespace DrawnUi.Maui.Draw;

public interface IDrawnUiPlatform
{
    Task<SKBitmap> LoadImageOnPlatformAsync(ImageSource source, CancellationToken cancel);

    void ClearImagesCache();

    /// <summary>
    /// Register to get invoked by plaform looper
    /// </summary>
    /// <param name="view"></param>
    void RegisterLooperCallback(EventHandler callback);

    /// <summary>
    /// Register to get invoked by plaform looper
    /// </summary>
    /// <param name="view"></param>
    void UnregisterLooperCallback(EventHandler callback);

    bool CheckNativeVisibility(object handler);

}