#if IOS || MACCATALYST
using Foundation;
using UIKit;

namespace DrawnUi.Camera;

public partial class SkiaCamera
{
    public virtual void TurnOnFlash()
    {
        NativeControl?.TurnOnFlash();
    }

    public virtual void TurnOffFlash()
    {
        NativeControl?.TurnOffFlash();
    }

    public virtual void SetZoom(double value)
    {
        //Set texture scale for digital zoom beyond hardware limits
        TextureScale = value;

        //Apply hardware zoom through native control
        NativeControl?.SetZoom((float)value);

        //Update display zoom - preview is our texture
        if (Display != null)
        {
            Display.ZoomX = TextureScale;
            Display.ZoomY = TextureScale;
        }

        Zoomed?.Invoke(this, value);
    }

    public virtual Metadata CreateMetadata()
    {
        return new Metadata()
        {
#if IOS
            Software = "SkiaCamera iOS",
#elif MACCATALYST
            Software = "SkiaCamera MacCatalyst",
#endif
            Vendor = UIDevice.CurrentDevice.Model,
            Model = UIDevice.CurrentDevice.Name,
        };
    }

    protected virtual void CreateNative()
    {
        if (!IsOn || NativeControl != null)
            return;

        NativeControl = new NativeCamera(this);
    }

    /// <summary>
    /// Call on UI thread only
    /// </summary>
    /// <returns></returns>
    public async Task<bool> RequestPermissions()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        return status == PermissionStatus.Granted;
    }


    public SKBitmap GetPreviewBitmap()
    {
        var preview = NativeControl?.GetPreviewImage();
        if (preview?.Image != null)
        {
            return SKBitmap.FromImage(preview.Image);
        }
        return null;
    }
}
#endif
