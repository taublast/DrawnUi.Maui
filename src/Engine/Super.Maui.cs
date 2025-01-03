using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace DrawnUi.Maui.Draw
{
    public partial class Super
    {
        public static Color ColorAccent { get; set; } = Colors.Orange;

        public static Color ColorPrimary { get; set; } = Colors.Gray;

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
                        TextColor = Colors.Red,
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
                        TextColor = Colors.Red,
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

        public static void Log(string message, LogLevel logLevel = LogLevel.Error, [CallerMemberName] string caller = null)
        {
            if (DrawnExtensions.StartupSettings != null)
            {
                DrawnExtensions.StartupSettings.Logger?.Log(logLevel, message);
            }

#if WINDOWS
        Trace.WriteLine(message);
#else
            Console.WriteLine(message);
#endif
        }


        public static IServiceProvider Services
        {
            get
            {
                var services = AppContext?.Services;
                if (services == null)
                {
                    services =
#if WINDOWS10_0_17763_0_OR_GREATER
            MauiWinUIApplication.Current.Services;
#elif ANDROID
            MauiApplication.Current.Services;
#elif IOS || MACCATALYST
            MauiUIApplicationDelegate.Current.Services;
#else
                        null;
#endif
                }
                return services;
            }
        }

        public static IMauiContext AppContext => Application.Current?.FindMauiContext();

#if WINDOWS

    // Win32 API constants and functions
    private const int GWL_STYLE = -16;
    private const int WS_THICKFRAME = 0x00040000;

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public static void MakeWindowNonResizable(IntPtr hWnd)
    {
        // Get the current window style
        IntPtr style = GetWindowLongPtr(hWnd, GWL_STYLE);

        // Remove the resize border (thick frame) from the style
        style = new IntPtr(style.ToInt64() & ~WS_THICKFRAME);

        // Set the modified style
        SetWindowLongPtr(hWnd, GWL_STYLE, style);
    }

#endif

        /// <summary>
        /// For desktop platforms, will resize app window and eventually lock it from being resized.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="isFixed"></param>
        public static void ResizeWindow(Window window, int width, int height, bool isFixed)
        {


            var disp = DeviceDisplay.Current.MainDisplayInfo;
            // move to screen center
            var x = (disp.Width / disp.Density - window.Width) / 2;
            var y = (disp.Height / disp.Density - window.Height) / 2;

            //this crashes in NET8 for CATALYST so..
#if !MACCATALYST

            window.Width = width;
            window.Height = height;
            window.X = x;
            window.Y = y;

#else

        var platformWindow = window.Handler?.PlatformView as UIKit.UIWindow;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var frame = new CoreGraphics.CGRect(x, y, platformWindow.Frame.Width, platformWindow.Frame.Height);

            platformWindow.Frame = frame;

            var windowScene = UIKit.UIApplication.SharedApplication.ConnectedScenes.ToArray().First() as UIKit.UIWindowScene;

            platformWindow.WindowScene.KeyWindow.Frame = frame;

            windowScene.RequestGeometryUpdate(
                new UIKit.UIWindowSceneGeometryPreferencesMac(frame),
                error =>
                {
                    var stopp = 1;
                });
        });

#endif

#if WINDOWS

        if (isFixed)
        {
            var platformWindow = window.Handler?.PlatformView as Microsoft.Maui.MauiWinUIWindow;
            var hWnd = platformWindow.WindowHandle;
            MakeWindowNonResizable(hWnd);
        }

#elif MACCATALYST

        foreach (var scene in UIKit.UIApplication.SharedApplication.ConnectedScenes)
        {
            if (scene is UIKit.UIWindowScene windowScene)
            {
                if (isFixed)
                {
                    //windowScene.SizeRestrictions.AllowsFullScreen = false;
                    //windowScene.SizeRestrictions.MinimumSize = new(width, height);
                    //windowScene.SizeRestrictions.MaximumSize = new(width, height);

                    var scale = windowScene.Screen.Scale;

                    // Tasks.StartDelayed(TimeSpan.FromSeconds(3),()=>
                    // {
                    //todo move to view appeared etc
                    // MainThread.BeginInvokeOnMainThread(()=>
                    // {
                    //     windowScene.RequestGeometryUpdate(
                    //         new UIKit.UIWindowSceneGeometryPreferencesMac(frame),
                    //         error =>
                    //         {
                    //             var stopp=1;
                    //         });
                    //     
                    //});

                    // });

                }

            }
        }

#endif

        }
    }
}
