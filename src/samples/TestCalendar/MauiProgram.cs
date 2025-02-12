using DrawnUi.Maui.Draw;
using Microsoft.Extensions.Logging;

namespace TestCalendar;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseDrawnUi(new()
            {
                MobileIsFullscreen = true,
                DesktopWindow = new()
                {
                    Height = 700,
                    Width = 400,
                    //IsFixedSize = true
                }
            })
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                fonts.AddFont("OpenSans-Regular.ttf", "FontText");
                fonts.AddFont("OpenSans-Semibold.ttf", "FontTextBold");

                fonts.AddFont("fa-regular-400.ttf", "Fa");
                fonts.AddFont("fa-light-300.ttf", "FaLight");
                fonts.AddFont("fa-solid-900.ttf", "FaSolid");
                fonts.AddFont("fa-brands-400.ttf", "FaBrands");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
