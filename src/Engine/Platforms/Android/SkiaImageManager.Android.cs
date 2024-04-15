using SkiaSharp.Views.Android;
using System.Diagnostics;

namespace DrawnUi.Maui.Draw;


public partial class SkiaImageManager
{
    public static bool UseGlide = true;

    public static async Task<SKBitmap> LoadImageOnPlatformAsync(ImageSource source, CancellationToken cancel)
    {
        if (source == null)
            return null;

        try
        {



            if (source is UriImageSource withUri)
            {
                var pfx = "native://";
                var url = withUri.Uri.ToString();
                if (url.Contains(pfx))
                {
                    var native = url.Replace(pfx, string.Empty).Trim('/');
                    source = new FileImageSource()
                    {
                        File = native
                    };
                    var handler = source.GetHandler();
                    var androidBitmap = await handler.LoadImageAsync(source, Platform.CurrentActivity, cancel);

                    if (androidBitmap != null)
                    {
                        SkiaImageManager.TraceLog($"[LoadImageOnPlatformAsync] loaded {source} ToSKBitmap");
                        return androidBitmap.ToSKBitmap();
                    }
                }
            }

            if (source is FileImageSource fileSource)
            {
                return await LoadFromFile(fileSource.File, cancel);
            }
            else
            if (UseGlide && source is UriImageSource)
            {
                var androidBitmap = await source.LoadOriginalViaGlide(Platform.CurrentActivity, cancel);
                if (androidBitmap != null)
                {
                    SkiaImageManager.TraceLog($"[LoadImageOnPlatformAsync-GLIDE] loaded {source} ToSKBitmap");
                    return androidBitmap.ToSKBitmap();
                }
            }
            else
            {
                var handler = source.GetHandler();
                var androidBitmap = await handler.LoadImageAsync(source, Platform.CurrentActivity, cancel);

                if (androidBitmap != null)
                {
                    SkiaImageManager.TraceLog($"[LoadImageOnPlatformAsync] loaded {source} ToSKBitmap");
                    return androidBitmap.ToSKBitmap();
                }
            }
        }
        catch (TaskCanceledException)
        {

        }
        catch (System.Exception e)
        {
            SkiaImageManager.TraceLog($"[LoadImageOnPlatformAsync] {e}");
        }

        return null;
    }

}
