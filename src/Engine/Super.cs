global using AppoMobi.Maui.Gestures;
global using AppoMobi.Specials;
global using DrawnUi.Maui.Draw;
global using DrawnUi.Maui.Extensions;
global using DrawnUi.Maui.Infrastructure;
global using DrawnUi.Maui.Infrastructure.Models;
global using DrawnUi.Maui.Models;
global using DrawnUi.Maui.Views;
global using SkiaSharp;
global using SkiaSharp.Views.Maui;
global using SkiaSharp.Views.Maui.Controls;
global using System.Diagnostics;
using System.Globalization;
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

#if (!ANDROID && !IOS && !MACCATALYST && !WINDOWS && !TIZEN)

    protected static void SetupFrameLooper()
    {
        throw new NotImplementedException();
    }

#endif

    /// <summary>
    /// Display xaml page creation exception
    /// </summary>
    /// <param name="view"></param>
    /// <param name="e"></param>
    public static void DisplayException(Element view, Exception e)
    {
        Trace.WriteLine(e);

        if (view == null)
            throw e;

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

    public static void Log(Exception e, [CallerMemberName] string caller = null)
    {
        //TODO use ILogger with levels etc

#if WINDOWS
        Trace.WriteLine(e);
#else
        Console.WriteLine(e);
#endif
    }

    public static void Log(string message, [CallerMemberName] string caller = null)
    {
        //TODO use ILogger with levels etc

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


    public static bool Initialized
    {
        get => initialized;
        set
        {
            initialized = value;
            EnableRendering = value;
        }
    }

    public static bool EnableRendering
    {
        get => enableRendering;
        set
        {
            enableRendering = value;
            if (value)
                NeedGlocalUpdate();
        }
    }

    private static bool _isRtl = false;
    /// <summary>
    /// RTL support UNDER CONSTRUCTION
    /// </summary>
    public static bool IsRtl
    {
        get => _isRtl;
        set
        {
            if (_isRtl != value)
            {
                _isRtl = value;
                NeedGlocalUpdate();
            }
        }
    }

    public static IApplication App { get; set; }

    /// <summary>
    /// Subscribe your navigation bar to react
    /// </summary>
    public static EventHandler InsetsChanged;

    private static IServiceProvider _services;
    private static bool _servicesFromHandler;

    public static IServiceProvider Services
        =>
#if WINDOWS10_0_17763_0_OR_GREATER
            MauiWinUIApplication.Current.Services;
#elif ANDROID
        MauiApplication.Current.Services;
#elif IOS || MACCATALYST
        MauiUIApplicationDelegate.Current.Services;
#else
        null;
#endif

    private static Screen _screen;
    public static Screen Screen
    {
        get
        {
            if (_screen == null)
                _screen = new Screen();
            return _screen;
        }
    }

    /// <summary>
    /// Capping FPS, (1 / FPS * 1000_000) 
    /// </summary>
    public static float CapMicroSecs = 8333.333333f;

    public static long GetCurrentTimeMs()
    {
        double timestamp = Stopwatch.GetTimestamp();
        double nanoseconds = 1_000.0 * timestamp / Stopwatch.Frequency;
        return (long)nanoseconds;
    }

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

    public static Color ColorAccent { get; set; } = Colors.Orange;
    public static Color ColorPrimary { get; set; } = Colors.Gray;

    public static double MaxFrameLengthMs { get; set; }

    /// <summary>
    /// App was launched and UI is ready to be created
    /// </summary>
    public static event EventHandler OnNativeAppCreated;

    public static event EventHandler OnNativeAppPaused;

    public static event EventHandler OnNativeAppResumed;

    public static event EventHandler OnNativeAppDestroyed;

    public static Size InitialWindowSize;

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

    static bool _attachedToWindow;

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
        MainThread.BeginInvokeOnMainThread(()=>
        {
            var frame = new CoreGraphics.CGRect(x, y,platformWindow.Frame.Width,platformWindow.Frame.Height);
            
            platformWindow.Frame = frame;
            
            var windowScene = UIKit.UIApplication.SharedApplication.ConnectedScenes.ToArray().First() as UIKit.UIWindowScene;
            
            platformWindow.WindowScene.KeyWindow.Frame = frame;
            
            windowScene.RequestGeometryUpdate(
                new UIKit.UIWindowSceneGeometryPreferencesMac(frame),
                error =>
                {
                    var stopp=1;
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

    public static void OnCreated()
    {
        InBackground = false;

        OnNativeAppCreated?.Invoke(null, EventArgs.Empty);
    }

    public static void OnWentBackground()
    {
        InBackground = true;
        OnNativeAppPaused?.Invoke(null, EventArgs.Empty);
    }

    public static void OnWentForeground()
    {
        InBackground = false;
        NeedGlocalUpdate();
        OnNativeAppResumed?.Invoke(null, EventArgs.Empty);
    }

    /// <summary>
    /// Maui App was launched and UI is ready to be consumed
    /// </summary>
    public static Action OnMauiAppCreated;


    public static event EventHandler NeedGlobalRefresh;

    public static void NeedGlocalUpdate()
    {
        NeedGlobalRefresh?.Invoke(null, null);
    }

    static RestartingTimer<object> _timerStopRenderingInBackground;

    private static bool _inBackground;
    private static bool initialized;
    private static bool enableRendering;

    public static bool InBackground
    {
        get
        {
            return _inBackground;
        }
        protected set
        {
            if (_inBackground != value)
            {
                _inBackground = value;
                if (value)
                {
                    SetStopRenderingInBackgroundWithDelay(2000);
                }
                else
                {
                    StopRenderingInBackground = false;
                }
            }
        }
    }

    static void SetStopRenderingInBackgroundWithDelay(int ms)
    {
        if (_timerStopRenderingInBackground == null)
        {
            _timerStopRenderingInBackground = new(TimeSpan.FromMilliseconds(ms), (arg) =>
            {
                if (InBackground)
                    StopRenderingInBackground = true;
            });
            _timerStopRenderingInBackground.Start(null);
        }
        else
        {
            _timerStopRenderingInBackground.Restart(null);
        }
    }

    public static bool StopRenderingInBackground { get; set; }

    private static void OnVisualTreeChanged(object sender, VisualTreeChangeEventArgs e)
    {
        //#if DEBUG
        //            Trace.WriteLine($"[VisualTreeChanged] parent {e.Parent} child {e.Child} index {e.ChildIndex}");
        //#endif

        //int failures = 0;
        //var parentSourInfo = e.Parent == null ? null : VisualDiagnostics.GetSourceInfo(e.Parent);
        //var childSourceInfo = VisualDiagnostics.GetSourceInfo(e.Child);
        //if (childSourceInfo == null)
        //    failures++;
        //if (e.Parent != null && parentSourInfo == null)
        //    failures++;
        //if (e.Parent != null && e.ChildIndex == -1)
        //    failures++;

        //if (failures > 0)
        //    Trace.WriteLine($"[VisualTreeChanged] failures: {failures}");

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RunOnMainThreadAndWait(Action action, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource();
        MainThread.InvokeOnMainThreadAsync(() =>
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                action();
                tcs.SetResult();
            }
            catch (OperationCanceledException)
            {
                tcs.TrySetCanceled(cancellationToken);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });
        tcs.Task.Wait(cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task RunOnMainThreadAndWaitAsync(Func<Task> asyncAction, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (MainThread.IsMainThread)
        {
            await asyncAction();
        }
        else
        {
            var tcs = new TaskCompletionSource<bool>();
            // Register a callback to handle cancellation
            using (cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken)))
            {
                MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await asyncAction();
                        tcs.TrySetResult(true);
                    }
                    catch (OperationCanceledException)
                    {
                        tcs.TrySetCanceled(cancellationToken);
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                });

                await tcs.Task.ConfigureAwait(false);
            }
        }
    }

    public static bool GpuCacheEnabled { get; set; } = true;

    public static string UserAgent { get; set; } = "Mozilla/5.0 AppleWebKit Chrome Mobile Safari";

#if SKIA3
    public static int SkiaGeneration = 3;
#else
    public static int SkiaGeneration = 2;
#endif
}