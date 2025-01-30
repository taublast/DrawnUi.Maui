//global using AppoMobi.Maui.Gestures;
//global using AppoMobi.Specials;
//global using SkiaSharp.Views.Maui;
//global using SkiaSharp.Views.Maui.Controls;
global using AppoMobi.Specials;
global using DrawnUi.Maui.Draw;
global using DrawnUi.Maui.Extensions;
global using DrawnUi.Maui.Infrastructure;
global using DrawnUi.Maui.Models;
global using SkiaSharp;
global using System.Collections.Generic;
global using System.Diagnostics;
global using Xamarin.Forms;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: XmlnsDefinition("http://schemas.appomobi.com/drawnUi/2023/draw",
    "DrawnUi.Maui.Draw")]

[assembly: XmlnsDefinition("http://schemas.appomobi.com/drawnUi/2023/draw",
    "DrawnUi.Maui.Controls")]

[assembly: XmlnsDefinition("http://schemas.appomobi.com/drawnUi/2023/draw",
    "DrawnUi.Maui.Views")]

namespace DrawnUi.Maui.Draw;

public partial class Super
{
    /// <summary>
    /// Can optionally disable hardware-acceleration with this flag, for example on iOS you would want to avoid creating many metal views.
    /// </summary>
    public static bool CanUseHardwareAcceleration = true;

    public static Assembly AppAssembly { get; set; }

    static IDrawnUiPlatform m_Record;
    public static IDrawnUiPlatform Native
    {
        get
        {
            if (m_Record == null)
                m_Record = DependencyService.Get<IDrawnUiPlatform>();
            return m_Record;
        }
    }

    /// <summary>
    /// Display xaml page creation exception
    /// </summary>
    /// <param name="view"></param>
    /// <param name="e"></param>
    public static void DisplayException(VisualElement view, Exception e)
    {
        Trace.WriteLine(e);

        if (view == null)
            throw e;

        if (view is SkiaControl skia)
        {
            var scroll = new SkiaScroll()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Content = new SkiaLabel
                {
                    Margin = new Thickness(32),
                    TextColor = Color.Red,
                    Text = $"{e}"
                }
            };

            if (skia is ContentLayout content)
            {
                content.Content = scroll;
            }
            else
            {
                skia.AddSubView(scroll);
            }
        }
        else
        {

            var scroll = new ScrollView()
            {
                Content = new Label
                {
                    Margin = new Thickness(32),
                    TextColor = Color.Red,
                    Text = $"{e}"
                }
            };

            if (view is ContentPage page)
            {
                page.Content = scroll;
            }
            else
            if (view is ContentView contentView)
            {
                contentView.Content = scroll;
            }
            else
            if (view is Grid grid)
            {
                grid.Children.Add(scroll);
            }
            else
            if (view is StackLayout stack)
            {
                stack.Children.Add(scroll);
            }
            else
            {
                throw e;
            }
        }

    }

    public static void Log(Exception e, [CallerMemberName] string caller = null)
    {


#if WINDOWS
        Trace.WriteLine(e);
#else
        Console.WriteLine(e);
#endif
    }

    public static void Log(string message, [CallerMemberName] string caller = null)
    {


#if WINDOWS
        Trace.WriteLine(message);
#else
        Console.WriteLine(message);
#endif
    }

    public static void SetLocale(string lang)
    {
        var culture = CultureInfo.CreateSpecificCulture(lang);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }


    public static Application App { get; set; }

    /// <summary>
    /// Subscribe your navigation bar to react
    /// </summary>
    public static EventHandler InsetsChanged;

    private static IServiceProvider _services;
    private static bool _servicesFromHandler;




    /// <summary>
    /// Capping FPS, default is 8333.3333 (1 / FPS * 1000_000) for 120 FPS
    /// </summary>
    public static float CapMicroSecs = 8333.3333f;

    public static long GetCurrentTimeNanos()
    {
        double timestamp = Stopwatch.GetTimestamp();
        double nanoseconds = 1_000_000_000.0 * timestamp / Stopwatch.Frequency;
        return (long)nanoseconds;
    }

    /// <summary>
    /// In DP
    /// </summary>
    public static double NavBarHeight { get; set; } = -1;

    /// <summary>
    /// In DP
    /// </summary>
    public static double StatusBarHeight { get; set; }

    /// <summary>
    /// In DP
    /// </summary>
    public static double BottomTabsHeight { get; set; } = 56;

    public static double BottomInsets { get; set; }

    public static Color ColorAccent { get; set; } = Color.Orange;
    public static Color ColorPrimary { get; set; } = Color.Gray;

    private static bool _enableRendering = true;
    public static bool EnableRendering
    {
        get => _enableRendering;
        set
        {
            _enableRendering = value;
            if (value)
                NeedGlobalUpdate();
        }
    }

    /// <summary>
    /// This will force recalculate canvas visibility in ViewTree and update those visible
    /// </summary>
    public static void NeedGlobalUpdate()
    {
        NeedGlobalRefresh?.Invoke(null, null);
    }

    public static event EventHandler NeedGlobalRefresh;


    public static void NeedUpdateLayout()
    {
        InsetsChanged?.Invoke(null, null);
    }

    public static bool GpuCacheEnabled { get; set; } = true;

    public static string UserAgent { get; set; } = "Mozilla/5.0 AppleWebKit Chrome Mobile Safari";


}