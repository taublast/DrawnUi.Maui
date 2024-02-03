using DrawnUi.Maui.Draw;
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;

namespace DrawnUi.Maui.Draw;

public static class WindowsExtensions
{

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
		if (source is FileImageSource)
		{
			returnValue = new FileImageSourceHandler();
		}
		else if (source is FontImageSource)
		{
			returnValue = new FontImageSourceHandler();
		}
		return returnValue;
	}

}