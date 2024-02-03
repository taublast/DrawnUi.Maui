using SkiaSharp.Views.iOS;
using System.Diagnostics;
using UIKit;

namespace DrawnUi.Maui.Draw;

public partial class SkiaImageManager
{
    public static async Task<SKBitmap> LoadSKBitmapAsync(ImageSource source, CancellationToken cancel)
    {
        if (source == null)
            return null;

        try
        {
            if (source is FileImageSource fileSource)
            {
                return await LoadFromFile(fileSource.File, cancel);
            }
            else
            {
                var iosImage = await LoadNativeImage(source, cancel, 1.0f);
                if (iosImage != null)
                {
                    return iosImage.ToSKBitmap();
                }
            }
        }
        catch (TaskCanceledException)
        {

        }
        catch (Exception e)
        {
            SkiaImageManager.TraceLog($"[LoadSKBitmapAsync] {e}");
        }

        return null;
    }

    public static async Task<UIImage> LoadNativeImage(ImageSource source, CancellationToken token, float scale)
    {
        if (source == null)
            return null;

        try
        {

            var handler = source.GetHandler();
            return await handler.LoadImageAsync(source, token);

        }
        catch (Exception e)
        {
            SkiaImageManager.TraceLog($"[LoadSKBitmapAsync] {e}");
        }

        return null;
    }

}