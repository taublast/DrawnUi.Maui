//global using AppoMobi.Maui.Gestures;
//global using AppoMobi.Specials;
//global using SkiaSharp.Views.Maui;
//global using SkiaSharp.Views.Maui.Controls;
global using System.Collections.Generic;
global using System.Diagnostics;
global using AppoMobi.Specials;
global using DrawnUi.Maui.Draw;
global using DrawnUi.Maui.Extensions;
global using DrawnUi.Maui.Infrastructure;
global using DrawnUi.Maui.Models;
global using SkiaSharp;
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


    public static void Log(string message, [CallerMemberName] string caller = null)
    {


#if WINDOWS
        Trace.WriteLine(message);
#else
        Console.WriteLine(message);
#endif
    }



    public static Application App { get; set; }




    public static double BottomInsets { get; set; }

    public static Color ColorAccent { get; set; } = Color.Orange;
    public static Color ColorPrimary { get; set; } = Color.Gray;




    public static void NeedUpdateLayout()
    {
        InsetsChanged?.Invoke(null, null);
    }



}
