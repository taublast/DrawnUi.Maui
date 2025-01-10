using Android.Graphics.Drawables;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;

namespace DrawnUi.Maui.Draw;

public static class AndroidExtensions
{
    public static bool IsNullOrDisposed(this Java.Lang.Object javaObject)
    {
        return javaObject == null || javaObject.Handle == IntPtr.Zero;
    }
    public static async Task<Drawable> GetDrawable(this ImageSource imageSource, IElementHandler handler)
    {
        IImageSourceServiceProvider imageSourceServiceProvider = handler.GetRequiredService<IImageSourceServiceProvider>();
        IImageSourceService service = imageSourceServiceProvider.GetImageSourceService(imageSource);
        var result = await service.GetDrawableAsync(imageSource, handler.MauiContext.Context);



        return result.Value;
    }
    public static IServiceProvider GetServiceProvider(this IElementHandler handler)
    {
        var context = handler.MauiContext ??
                      throw new InvalidOperationException($"Unable to find the context. The {nameof(ElementHandler.MauiContext)} property should have been set by the host.");

        var services = context?.Services ??
                       throw new InvalidOperationException($"Unable to find the service provider. The {nameof(ElementHandler.MauiContext)} property should have been set by the host.");

        return services;
    }

    public static T GetRequiredService<T>(this IElementHandler handler)
        where T : notnull
    {
        var services = handler.GetServiceProvider();

        var service = services.GetRequiredService<T>();

        return service;
    }

    public static IImageSourceHandler GetHandler(this ImageSource source)
    {
        //Image source handler to return
        IImageSourceHandler returnValue = null;
        //check the specific source type and return the correct image source handler
        if (source is UriImageSource)
        {
            returnValue = new ImageLoaderSourceHandler();
        }
        else if (source is FileImageSource)
        {
            returnValue = new FileImageSourceHandler();
        }
        else if (source is StreamImageSource)
        {
            returnValue = new StreamImagesourceHandler();
        }
        else if (source is FontImageSource)
        {
            returnValue = new FontImageSourceHandler();
        }
        return returnValue;
    }
}
