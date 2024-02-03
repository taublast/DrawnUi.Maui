using Microsoft.Maui.Storage;
using System.Diagnostics;
using System.Net;
using System.Security.Authentication;

namespace DrawnUi.Maui.Draw;

public partial class SkiaImageManager
{
    public async Task<SKBitmap> GetSKBitmapFromStreamImageSource(StreamImageSource streamImageSource)
    {
        var stream = await streamImageSource.Stream(default);
        var skStream = new SKManagedStream(stream);
        var codec = SKCodec.Create(skStream);
        var skBitmap = new SKBitmap(codec.Info);

        //codec.(skBitmap.Info, skBitmap.GetPixels(), skBitmap.RowBytes, 0, 0);

        return skBitmap;
    }

    public static async Task<SKBitmap> LoadSKBitmapAsync(ImageSource source, CancellationToken cancel)
    {
        if (source == null)
            return null;

        try
        {
            if (source is StreamImageSource streamSource)
            {
                using (var stream = await streamSource.Stream(cancel))
                {
                    return SKBitmap.Decode(stream);
                }
            }
            else
            if (source is UriImageSource uriSource)
            {
                var httpClientHandler = new HttpClientHandler();
                if (httpClientHandler.SupportsAutomaticDecompression)
                {
                    httpClientHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                }

                //#if DEBUG
                //						httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                //						{
                //							return true;
                //						};
                //						httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                //#endif

                httpClientHandler.SslProtocols = SslProtocols.Tls12;
                var client = new HttpClient(httpClientHandler);
                var response = await client.GetAsync(uriSource.Uri, cancel);
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        return SKBitmap.Decode(stream);
                    }
                }

            }
            else
            if (source is FileImageSource fileSource)
            {
                return await LoadFromFile(fileSource.File, cancel);
            }
            else
            {
                throw new NotImplementedException();
            }

            //var handler = source.GetHandler();
            //if (handler != null)
            //{
            //	//case of FileImageSourceHandler or FontImageSourceHandler
            //	var imageSource = await handler.LoadImageAsync(source, cancel);

            //	throw new NotImplementedException();
            //}

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



}