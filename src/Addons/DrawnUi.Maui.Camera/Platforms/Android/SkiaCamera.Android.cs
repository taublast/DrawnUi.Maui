using Android.Content;


namespace DrawnUi.Maui.Camera;

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
        //todo
        //since hardware zoom not supported on droid atm we set this here manually
        TextureScale = value;

        //in theory nativecontrol should set TextureScale regarding on the amount it was able to set using hardware
        //so the remaining zoom comes from scaling the output texture (preview)
        NativeControl.SetZoom((float)value);

        //temporary hack - preview is our texture
        Display.ZoomX = TextureScale;
        Display.ZoomY = TextureScale;

        Zoomed?.Invoke(this, value);
    }


    public void OpenFileInGallery(string imageFilePath)
    {
        // Create a new Intent
        Intent intent = new Intent();

        // Set the Intent action to ACTION_VIEW (view the data)
        intent.SetAction(Intent.ActionView);

        // Create a Uri from the file path
        var photoUri = FileProvider.GetUriForFile(Platform.AppContext, Platform.AppContext.PackageName + ".provider", new Java.IO.File(imageFilePath));

        // Set the Intent data to the Uri
        intent.SetDataAndType(photoUri, "image/*");

        // Add this line to give temporary permission to the external app to use your FileProvider
        intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.GrantReadUriPermission);

        // Start the external activity
        Platform.AppContext.StartActivity(intent);
    }

    public virtual Metadata CreateMetadata()
    {
        return new Metadata()
        {
            Software = "SkiaCamera Android",
            Vendor = $"{Android.OS.Build.Manufacturer}",
            Model = $"{Android.OS.Build.Model}"
        };
    }



    protected virtual void CreateNative()
    {
        if (!IsOn || NativeControl != null)
            return;

        //DisableOtherCameras();

        NativeControl = new NativeCamera(this);

        //OnUpdateOrientation(null, null);

        //SubscribeToNativeControl();
    }


    /// <summary>
    /// Call on UI thread only
    /// </summary>
    /// <returns></returns>
    public async Task<bool> RequestPermissions()
    {
        var status = await Permissions
            .CheckStatusAsync<Permissions.Camera>();

        return status == PermissionStatus.Granted;
    }



}
