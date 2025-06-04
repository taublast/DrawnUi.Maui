using System.Diagnostics;

namespace DrawnUi.Camera;

public partial class SkiaCamera : SkiaControl
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
            Software = "SkiaCamera Windows",
            Vendor = Environment.MachineName,
            Model = Environment.OSVersion.ToString(),
        };
    }

    protected virtual void CreateNative()
    {
        if (!IsOn || NativeControl != null)
        {
            Debug.WriteLine($"[SkiaCameraWindows] CreateNative skipped - IsOn: {IsOn}, NativeControl exists: {NativeControl != null}");
            return;
        }

        Debug.WriteLine("[SkiaCameraWindows] Creating native camera...");
        NativeControl = new NativeCamera(this);
        Debug.WriteLine("[SkiaCameraWindows] Native camera created");
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

    public void CommandToRenderer(string command)
    {
        // Implementation for renderer commands if needed
        // This can be expanded based on requirements
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
