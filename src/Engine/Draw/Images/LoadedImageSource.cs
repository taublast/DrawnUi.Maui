namespace DrawnUi.Maui.Draw;

public class LoadedImageSource : IDisposable
{
    public LoadedImageSource Clone()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException("Cannot clone a disposed LoadedImageSource");
        }

        if (Bitmap != null)
        {
            // Clone the SKBitmap
            var bitmapClone = new SKBitmap(Bitmap.Width, Bitmap.Height, Bitmap.ColorType, Bitmap.AlphaType);
            Bitmap.CopyTo(bitmapClone);
            return new LoadedImageSource(bitmapClone);
        }
        else if (Image != null)
        {
            // Clone the SKImage
            var imageClone = SKImage.FromBitmap(SKBitmap.FromImage(Image));
            return new LoadedImageSource(imageClone);
        }
        else
        {
            // If there's no image or bitmap, return a new empty instance
            return new LoadedImageSource();
        }
    }

    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// As this can be disposed automatically by the consuming control like SkiaImage etc we can manually prohibit this for cases this instance is used elsewhere. 
    /// </summary>
    public bool ProtectFromDispose { get; set; }

    public void Dispose()
    {
        if (!IsDisposed && !ProtectFromDispose)
        {
            IsDisposed = true;

            Bitmap?.Dispose();
            Bitmap = null;
            Image?.Dispose();
            Image = null;
        }
    }

    public LoadedImageSource(SKBitmap bitmap)
    {
        Bitmap = bitmap;
    }

    public LoadedImageSource(SKImage image)
    {
        Image = image;
    }

    public LoadedImageSource(byte[] bytes)
    {
        Bitmap = SKBitmap.Decode(bytes);
    }

    public LoadedImageSource()
    {

    }

    public int Width => Bitmap?.Width ?? Image?.Width ?? 0;
    public int Height => Bitmap?.Height ?? Image?.Height ?? 0;

    public bool IsDisposed { get; protected set; }

    public SKBitmap Bitmap { get; set; }
    public SKImage Image { get; set; }

    public SKBitmap GetBitmap()
    {
        if (Bitmap != null)
        {
            return Bitmap;
        }
        if (Image != null)
        {
            return SKBitmap.FromImage(Image);
        }
        return null;
    }
}