namespace DrawnUi.Draw;

public interface IRefreshIndicator : IDrawnBase
{
    /// <summary>
    /// 0 - 1
    /// </summary>
    /// <param name="ratio"></param>
    void SetDragRatio(float ratio);
 

}