namespace DrawnUi.Maui.Draw;

public interface ICanBeUpdated
{
    /// <summary>
    /// Force redrawing, without invalidating the measured size
    /// </summary>
    /// <returns></returns>
    void Update();

}