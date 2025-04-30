namespace DrawnUi.Draw;

public enum HardwareAccelerationMode
{
    /// <summary>
    /// Default
    /// </summary>
    Disabled,

    /// <summary>
    /// Gestures attached
    /// </summary>
    Enabled,

    /// <summary>
    /// A non-accelerated view will be created first to avoid blank screen while graphic context is being initialized, then swapped with accelerated view
    /// </summary>
    Prerender,
}