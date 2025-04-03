namespace DrawnUi.Draw;

public interface IInsideViewport : IVisibilityAware, IDisposable
{
    public void OnViewportWasChanged(ScaledRect viewport);

    /// <summary>
    /// IInsideViewport interface: loaded is called when the view is created, but not yet visible
    /// </summary>
    void OnLoaded();
}