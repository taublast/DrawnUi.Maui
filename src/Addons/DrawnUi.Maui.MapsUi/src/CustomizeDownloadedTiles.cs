using BruTile;
using BruTile.Cache;
using BruTile.Web;
using DrawnUi.Maui.Draw;
using SkiaSharp;

namespace DrawnUi.Maui.MapsUi;

public class CustomizeDownloadedTiles : HttpTileSource
{
    public CustomizeDownloadedTiles(ITileSchema tileSchema, string urlFormatter, IEnumerable<string> serverNodes = null, string apiKey = null, string name = null, IPersistentCache<byte[]> persistentCache = null, Func<Uri, Task<byte[]>> tileFetcher = null, Attribution attribution = null, string userAgent = null) : base(tileSchema, urlFormatter, serverNodes, apiKey, name, persistentCache, tileFetcher, attribution, userAgent)
    {

    }

    public CustomizeDownloadedTiles(ITileSchema tileSchema, IRequest request, string name = null, IPersistentCache<byte[]> persistentCache = null, Func<Uri, Task<byte[]>> tileFetcher = null, Attribution attribution = null, string userAgent = null) : base(tileSchema, request, name, persistentCache, tileFetcher, attribution, userAgent)
    {

    }

    public Action<Action<SKPaint>, SKCanvas> RenderBitmap;

    public override async Task<byte[]> GetTileAsync(TileInfo tileInfo)
    {
        var data = await base.GetTileAsync(tileInfo);

        if (RenderBitmap != null)
        {
            using var skData = SKData.CreateCopy(data);
            using var image = SKImage.FromEncodedData(skData);
            using var bitmap = SKBitmap.FromImage(image);

            using var surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height));
            var canvas = surface.Canvas;

            RenderBitmap.Invoke((paint) =>
            {
                canvas.DrawBitmap(bitmap, new SKRect(0, 0, bitmap.Width, bitmap.Height), paint);
            },
                canvas);

            using var modifiedImage = surface.Snapshot();
            using var modifiedData = modifiedImage.Encode();
            data = modifiedData.ToArray();
        }

        return data;
    }



}