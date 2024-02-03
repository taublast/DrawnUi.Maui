namespace DrawnUi.Maui.Draw;

public interface IInsideViewport : IVisibilityAware, IDisposable
{
    public void OnViewportWasChanged(ScaledRect viewport);

    /// <summary>
    /// Loaded is called when the view is created, but not yet visible
    /// </summary>
    void OnLoaded();
}