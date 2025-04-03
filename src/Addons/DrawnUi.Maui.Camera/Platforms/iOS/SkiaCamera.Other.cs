using DrawnUi.Draw;
using SkiaSharp;

namespace DrawnUi.Camera;

public partial class SkiaCamera : SkiaControl
{
    public virtual void SetZoom(double value)
    {

    }
    /// <summary>
    /// Call on UI thread only
    /// </summary>
    /// <returns></returns>
    public async Task<bool> RequestPermissions()
    {
        return true;
    }

    public void CommandToRenderer(string command)
    {
        throw new NotImplementedException();
    }

    public SKBitmap GetPreviewBitmap()
    {
        throw new NotImplementedException();
    }


}
