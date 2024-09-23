namespace DrawnUi.Maui.Draw;

public class CachedObject : IDisposable
{
    public SKPoint TranslateInputCoords(SKRect drawingRect)
    {
        //return new(0, 0);

        var offsetCacheX = drawingRect.Left - Bounds.Left;
        var offsetCacheY = drawingRect.Top - Bounds.Top;

        return new SKPoint(-offsetCacheX, -offsetCacheY);
    }

    public SKPoint CalculatePositionOffset(SKPoint drawingRect)
    {
        //return new(0, 0);

        var offsetCacheX = drawingRect.X - Bounds.Left;
        var offsetCacheY = drawingRect.Y - Bounds.Top;

        //var offsetCacheX = (float)Math.Round(drawingRect.X - Bounds.Left);
        //var offsetCacheY = (float)Math.Round(drawingRect.Y - Bounds.Top);


        return new SKPoint(offsetCacheX, offsetCacheY);
    }

    public SKPoint Test(SKRect drawingRect)
    {

        var offsetCacheX = Math.Abs(drawingRect.Left - Bounds.Left);
        var offsetCacheY = Math.Abs(drawingRect.Top - Bounds.Top);

        return new SKPoint(offsetCacheX, offsetCacheY);
    }

    public void Draw(SKCanvas canvas, SKRect destination, SKPaint paint)
    {
        try
        {
            if (Picture != null)
            {
                //var x = destination.Left - Bounds.Left;
                //var y = destination.Top - Bounds.Top;

                var x = (float)Math.Round(destination.Left - Bounds.Left);
                var y = (float)Math.Round(destination.Top - Bounds.Top);

                if (Type == SkiaCacheType.OperationsFull)
                {
                    var stop = 1;
                    //x = (float)Math.Round(RecordingArea.Left);
                    //y = (float)Math.Round(RecordingArea.Top);
                }

                canvas.DrawPicture(Picture, x, y, paint);
            }
            else
            if (Image != null)
            {
                //var x = destination.Left;
                //var y = destination.Top;

                var x = (float)Math.Round(destination.Left);
                var y = (float)Math.Round(destination.Top);

                canvas.DrawImage(Image, x, y, paint);
            }
        }
        catch (Exception e)
        {
            Super.Log(e); //on windows we had an issue when refreshing image tab 2/2 of demo
        }
    }

    public CachedObject(SkiaCacheType type, SKPicture picture, SKRect bounds, SKRect recordingArea)
    {
        Type = type;
        Bounds = bounds;
        RecordingArea = recordingArea;
        Picture = picture;
    }

    public CachedObject(SkiaCacheType type, SKSurface surface, SKRect bounds, SKRect recordingArea)
    {
        Type = type;
        Surface = surface;
        Bounds = bounds;
        RecordingArea = recordingArea;
        Image = surface.Snapshot();


    }

    /// <summary>
    /// An existing surface was reused for creating this object
    /// </summary>
    public bool SurfaceIsRecycled { get; set; }

    public SKPicture Picture { get; set; }

    public SKImage Image { get; set; }

    public SKRect Bounds { get; set; }

    public SKRect RecordingArea { get; set; }

    public SkiaCacheType Type { get; protected set; }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;

            Surface?.Dispose();
            Surface = null;
            Picture?.Dispose();
            Picture = null;
            Image?.Dispose();
            Image = null;
        }
    }

    public bool IsDisposed { get; protected set; }

    public string Tag { get; set; }

    public bool NeedDispose { get; set; }

    public SKBitmap GetBitmap()
    {
        return SKBitmap.FromImage(Image);
    }

    public SKSurface Surface { get; set; }
}