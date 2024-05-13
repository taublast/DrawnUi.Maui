using BruTile;
using BruTile.Cache;
using BruTile.Web;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia;
using Mapsui.Rendering.Skia.Extensions;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using SkiaSharp;
using System.Diagnostics.CodeAnalysis;
using Color = Mapsui.Styles.Color;

namespace Mapsui.Samples.Maui;

public class CustomTilesSource : HttpTileSource
{
    public CustomTilesSource(ITileSchema tileSchema, string urlFormatter, IEnumerable<string> serverNodes = null, string apiKey = null, string name = null, IPersistentCache<byte[]> persistentCache = null, Func<Uri, Task<byte[]>> tileFetcher = null, Attribution attribution = null, string userAgent = null) : base(tileSchema, urlFormatter, serverNodes, apiKey, name, persistentCache, tileFetcher, attribution, userAgent)
    {

    }

    public CustomTilesSource(ITileSchema tileSchema, IRequest request, string name = null, IPersistentCache<byte[]> persistentCache = null, Func<Uri, Task<byte[]>> tileFetcher = null, Attribution attribution = null, string userAgent = null) : base(tileSchema, request, name, persistentCache, tileFetcher, attribution, userAgent)
    {

    }

    public override async Task<byte[]> GetTileAsync(TileInfo tileInfo)
    {
        var data = await base.GetTileAsync(tileInfo);

        //using var skData = SKData.CreateCopy(data);
        //var image = SKImage.FromEncodedData(skData);
        //var bmp = new BitmapInfo { Bitmap = image };

        //todo modify image

        //codo convert back to data so it would be cached as modified

        return data;
    }
}

public class SkiaBitmapRenderer
{
    // The field below is static for performance. Effect has not been measured.
    // Note that the default FilterQuality is None. Setting it explicitly to Low increases the quality.
    private static readonly SKPaint DefaultPaint = new() { FilterQuality = SKFilterQuality.Low };

    public static void Draw(SKCanvas canvas, SKImage bitmap, SKRect rect, float layerOpacity = 1f)
    {
        var skPaint = GetPaint(layerOpacity, out var dispose);
        canvas.DrawImage(bitmap, rect, skPaint);
        if (dispose)
            skPaint.Dispose();
    }

    public static void Draw(SKCanvas canvas, SKImage? bitmap, float x, float y, float rotation = 0,
        float offsetX = 0, float offsetY = 0,
        LabelStyle.HorizontalAlignmentEnum horizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
        LabelStyle.VerticalAlignmentEnum verticalAlignment = LabelStyle.VerticalAlignmentEnum.Center,
        float opacity = 1f,
        float scale = 1f)
    {
        if (bitmap == null)
            return;

        canvas.Save();

        canvas.Translate(x, y);
        if (rotation != 0)
            canvas.RotateDegrees(rotation, 0, 0);
        canvas.Scale(scale, scale);

        var width = bitmap.Width;
        var height = bitmap.Height;

        x = offsetX + DetermineHorizontalAlignmentCorrection(horizontalAlignment, width);
        y = -offsetY + DetermineVerticalAlignmentCorrection(verticalAlignment, height);

        var halfWidth = width >> 1;
        var halfHeight = height >> 1;

        var rect = new SKRect(x - halfWidth, y - halfHeight, x + halfWidth, y + halfHeight);

        Draw(canvas, bitmap, rect, opacity);

        canvas.Restore();
    }
    private static int DetermineHorizontalAlignmentCorrection(
        LabelStyle.HorizontalAlignmentEnum horizontalAlignment, int width)
    {
        if (horizontalAlignment == LabelStyle.HorizontalAlignmentEnum.Left) return width >> 1;
        if (horizontalAlignment == LabelStyle.HorizontalAlignmentEnum.Right) return -(width >> 1);
        return 0; // center
    }

    private static int DetermineVerticalAlignmentCorrection(
        LabelStyle.VerticalAlignmentEnum verticalAlignment, int height)
    {
        if (verticalAlignment == LabelStyle.VerticalAlignmentEnum.Top) return -(height >> 1);
        if (verticalAlignment == LabelStyle.VerticalAlignmentEnum.Bottom) return height >> 1;
        return 0; // center
    }

    private static SKPaint GetPaint(float layerOpacity, out bool dispose)
    {
        if (Math.Abs(layerOpacity - 1) > Utilities.Constants.Epsilon)
        {
            // Unfortunately for opacity we need to set the Color and the Color
            // is part of the LinePaint object. So we need to recreate the paint on
            // every draw. 
            dispose = true;
            return new SKPaint
            {
                FilterQuality = SKFilterQuality.Low,
                Color = new SKColor(255, 255, 255, (byte)(255 * layerOpacity))
            };
        }
        dispose = false;
        return DefaultPaint;
    }
}

public class SkiaPictureRenderer
{
    // The field below is static for performance. Effect has not been measured.
    // Note that setting the FilterQuality to Low increases the quality because the default is None.
    private static readonly SKPaint DefaultPaint = new() { FilterQuality = SKFilterQuality.Low };

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP001:Dispose created")]
    public static void Draw(SKCanvas canvas, SKPicture picture, SKRect rect, float layerOpacity = 1f, Color? blendModeColor = null)
    {
        var skPaint = GetPaint(layerOpacity, blendModeColor, out var dispose);

        var scaleX = rect.Width / picture.CullRect.Width;
        var scaleY = rect.Height / picture.CullRect.Height;

        SKMatrix matrix;
        if (scaleX == 1 && scaleY == 1)
        {
            matrix = SKMatrix.CreateTranslation(rect.Left, rect.Top);
        }
        else
        {
            matrix = SKMatrix.CreateScaleTranslation(scaleX, scaleY, rect.Left, rect.Top);
        }

        canvas.DrawPicture(picture, ref matrix, skPaint);
        if (dispose)
        {
            skPaint.Dispose();
        }
    }

    public static void Draw(SKCanvas canvas, SKPicture? picture, float x, float y, float rotation = 0,
        float offsetX = 0, float offsetY = 0,
        LabelStyle.HorizontalAlignmentEnum horizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
        LabelStyle.VerticalAlignmentEnum verticalAlignment = LabelStyle.VerticalAlignmentEnum.Center,
        float opacity = 1f,
        float scale = 1f,
        Color? blendModeColor = null)
    {
        if (picture == null)
            return;

        canvas.Save();

        canvas.Translate(x, y);
        if (rotation != 0)
            canvas.RotateDegrees(rotation, 0, 0);
        canvas.Scale(scale, scale);

        var width = picture.CullRect.Width;
        var height = picture.CullRect.Height;

        x = offsetX + DetermineHorizontalAlignmentCorrection(horizontalAlignment, width);
        y = -offsetY + DetermineVerticalAlignmentCorrection(verticalAlignment, height);

        var halfWidth = width / 2;
        var halfHeight = height / 2;

        var rect = new SKRect(x - halfWidth, y - halfHeight, x + halfWidth, y + halfHeight);

        Draw(canvas, picture, rect, opacity, blendModeColor);

        canvas.Restore();
    }
    private static float DetermineHorizontalAlignmentCorrection(
        LabelStyle.HorizontalAlignmentEnum horizontalAlignment, float width)
    {
        if (horizontalAlignment == LabelStyle.HorizontalAlignmentEnum.Left) return width / 2;
        if (horizontalAlignment == LabelStyle.HorizontalAlignmentEnum.Right) return -(width / 2);
        return 0; // center
    }

    private static float DetermineVerticalAlignmentCorrection(
        LabelStyle.VerticalAlignmentEnum verticalAlignment, float height)
    {
        if (verticalAlignment == LabelStyle.VerticalAlignmentEnum.Top) return -(height / 2);
        if (verticalAlignment == LabelStyle.VerticalAlignmentEnum.Bottom) return height / 2;
        return 0; // center
    }

    [SuppressMessage("IDisposableAnalyzers.Correctness", "IDISP015:Member should not return created and cached instance")]
    private static SKPaint GetPaint(float layerOpacity, Color? blendModeColor, out bool dispose)
    {
        if (blendModeColor is not null)
        {
            // Unfortunately when blendModeColor is set we need to create a new SKPaint for
            // possible individually different color arguments. 
            dispose = true;
            return new SKPaint
            {
                FilterQuality = SKFilterQuality.Low,
                ColorFilter = SKColorFilter.CreateBlendMode(blendModeColor.ToSkia(layerOpacity), SKBlendMode.SrcIn)
            };
        };

        if (Math.Abs(layerOpacity - 1) > Utilities.Constants.Epsilon)
        {
            // Unfortunately when opacity is set we need to create a new SKPaint for
            // possible individually different opacity arguments. 
            dispose = true;
            return new SKPaint
            {
                FilterQuality = SKFilterQuality.Low,
                Color = new SKColor(255, 255, 255, (byte)(255 * layerOpacity))
            };
        };

        dispose = false;
        return DefaultPaint;
    }
}

public class DrawnUiRasterStyleRenderer : ISkiaStyleRenderer
{
    public bool Draw(SKCanvas canvas, Viewport viewport, ILayer layer, IFeature feature, IStyle style, IRenderCache renderCache, long currentIteration)
    {
        try
        {
            var rasterFeature = feature as RasterFeature;
            var raster = rasterFeature?.Raster;

            var opacity = (float)(layer.Opacity * style.Opacity);

            if (raster == null)
                return false;

            if (style is not RasterStyle)
                return false;

            renderCache.TileCache.UpdateCache(currentIteration);

            if (renderCache.TileCache.GetOrCreate(raster, currentIteration) is not BitmapInfo bitmapInfo)
                return false;

            var extent = feature.Extent;

            if (extent == null)
                return false;

            canvas.Save();

            if (viewport.IsRotated())
            {
                var priorMatrix = canvas.TotalMatrix;

                var matrix = CreateRotationMatrix(viewport, extent, priorMatrix);

                canvas.SetMatrix(matrix);

                var destination = new SKRect(0.0f, 0.0f, (float)extent.Width, (float)extent.Height);

                switch (bitmapInfo.Type)
                {
                case BitmapType.Bitmap:
                SkiaBitmapRenderer.Draw(canvas, bitmapInfo.Bitmap!, destination, opacity);
                break;
                case BitmapType.Picture:
                SkiaPictureRenderer.Draw(canvas, bitmapInfo.Picture!, destination, opacity);
                break;
                }

                canvas.SetMatrix(priorMatrix);
            }
            else
            {
                var destination = WorldToScreen(viewport, extent);
                switch (bitmapInfo.Type)
                {
                case BitmapType.Bitmap:
                SkiaBitmapRenderer.Draw(canvas, bitmapInfo.Bitmap!, RoundToPixel(destination), opacity);
                break;
                case BitmapType.Picture:
                SkiaPictureRenderer.Draw(canvas, bitmapInfo.Picture!, RoundToPixel(destination), opacity);
                break;
                }
            }

            canvas.Restore();
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, ex.Message, ex);
        }

        return true;
    }

    private static SKMatrix CreateRotationMatrix(Viewport viewport, MRect rect, SKMatrix priorMatrix)
    {
        // The front-end sets up the canvas with a matrix based on screen scaling (e.g. retina).
        // We need to retain that effect by combining our matrix with the incoming matrix.

        // We'll create four matrices in addition to the incoming matrix. They perform the
        // zoom scale, focal point offset, user rotation and finally, centering in the screen.

        var userRotation = SKMatrix.CreateRotationDegrees((float)viewport.Rotation);
        var focalPointOffset = SKMatrix.CreateTranslation(
            (float)(rect.Left - viewport.CenterX),
            (float)(viewport.CenterY - rect.Top));
        var zoomScale = SKMatrix.CreateScale((float)(1.0 / viewport.Resolution), (float)(1.0 / viewport.Resolution));
        var centerInScreen = SKMatrix.CreateTranslation((float)(viewport.Width / 2.0), (float)(viewport.Height / 2.0));

        // We'll concatenate them like so: incomingMatrix * centerInScreen * userRotation * zoomScale * focalPointOffset

        var matrix = SKMatrix.Concat(zoomScale, focalPointOffset);
        matrix = SKMatrix.Concat(userRotation, matrix);
        matrix = SKMatrix.Concat(centerInScreen, matrix);
        matrix = SKMatrix.Concat(priorMatrix, matrix);

        return matrix;
    }

    private static SKRect WorldToScreen(Viewport viewport, MRect rect)
    {
        var first = viewport.WorldToScreen(rect.Min.X, rect.Min.Y);
        var second = viewport.WorldToScreen(rect.Max.X, rect.Max.Y);
        return new SKRect
        (
            (float)Math.Min(first.X, second.X),
            (float)Math.Min(first.Y, second.Y),
            (float)Math.Max(first.X, second.X),
            (float)Math.Max(first.Y, second.Y)
        );
    }

    private static SKRect RoundToPixel(SKRect boundingBox)
    {
        return new SKRect(
            (float)Math.Round(boundingBox.Left),
            (float)Math.Round(Math.Min(boundingBox.Top, boundingBox.Bottom)),
            (float)Math.Round(boundingBox.Right),
            (float)Math.Round(Math.Max(boundingBox.Top, boundingBox.Bottom)));
    }
}