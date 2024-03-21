using Microsoft.Maui.Storage;
using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
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

    /*
    public static async Task<Microsoft.UI.Xaml.Media.ImageSource> LoadNativeImage(ImageSource source, CancellationToken token)
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

     
    public async Task<SKBitmap> ImageSourceToSKBitmap(Microsoft.UI.Xaml.Media.ImageSource source)
    {
        WriteableBitmap writeableBitmap = null;

        switch (source)
        {
            case BitmapImage bitmapImage:
                // Convert BitmapImage to WriteableBitmap if necessary
                // This step depends on having the BitmapImage already loaded and available
                // One approach is to draw the BitmapImage onto a WriteableBitmap
                throw new NotImplementedException("Conversion from BitmapImage to WriteableBitmap needs implementation based on specific context.");

            case RenderTargetBitmap renderTargetBitmap:
                // Directly use RenderTargetBitmap if this is the case
                writeableBitmap = new WriteableBitmap(renderTargetBitmap.PixelWidth, renderTargetBitmap.PixelHeight);
                await renderTargetBitmap.RenderAsync(writeableBitmap);
                break;

            case WriteableBitmap bmp:
                writeableBitmap = bmp;
                break;

            default:
                throw new ArgumentException("Unsupported ImageSource type");
        }

        if (writeableBitmap == null)
        {
            throw new InvalidOperationException("Could not convert ImageSource to WriteableBitmap.");
        }

        // Access pixel data
        using var stream = writeableBitmap.PixelBuffer.AsStream();
        var pixels = new byte[stream.Length];
        await stream.ReadAsync(pixels, 0, pixels.Length);

        // Create SKBitmap
        var skBitmap = new SKBitmap(writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, SKColorType.Bgra8888, SKAlphaType.Premul);
        skBitmap.InstallPixels(skBitmap.Info, pixels, writeableBitmap.PixelWidth * 4); // Assuming 4 bytes per pixel (BGRA8888)

        return skBitmap;
    }
    */

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