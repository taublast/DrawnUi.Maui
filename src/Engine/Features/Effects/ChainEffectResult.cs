namespace DrawnUi.Maui.Draw;

public struct ChainEffectResult
{
    /// <summary>
    /// Set this to true if you drawn the control of false if you just rendered something else
    /// </summary>
    public bool DrawnControl { get; set; }

    /// <summary>
    /// Set this to return value you got after saving a layer if any
    /// </summary>
    public int NeedRestoreToCount { get; set; }

    public static ChainEffectResult Create(bool drawnControl, int restore)
    {
        return new ChainEffectResult
        {
            DrawnControl = drawnControl,
            NeedRestoreToCount = restore
        };
    }

    public static ChainEffectResult Default
    {
        get
        {
            return new ChainEffectResult();
        }
    }
}