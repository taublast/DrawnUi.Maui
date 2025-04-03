namespace DrawnUi.Draw;

public interface ISkiaCell : IVisibilityAware
{
    // Cell went on screen inside parent viewport. This will be called after binding context was set
    //OnAppearing

    // Cell went offscreen from parent viewport
    // OnDisappeared

    public void OnScrolled();
}

public interface IVisibilityAware
{
    /// <summary>
    /// This can sometimes be omitted, 
    /// </summary>
    void OnAppearing();

    /// <summary>
    /// This event can sometimes be called without prior OnAppearing
    /// </summary>
    void OnAppeared();

    void OnDisappeared();

    void OnDisappearing();
}