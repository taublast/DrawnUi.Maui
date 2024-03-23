using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
//using Bumptech.Glide;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using static Android.Views.Choreographer;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace DrawnUi.Maui.Draw;

public partial class Super
{
    protected static void SetupFrameLooper()
    {
        Tasks.StartDelayed(TimeSpan.FromMilliseconds(1), async () =>
        {
            await StartFrameLooperAsync(CancellationToken.None);
        });
    }

    protected static async Task StartFrameLooperAsync(CancellationToken cancellationToken)
    {
        var frameStopwatch = new Stopwatch();
        var loopStopwatch = Stopwatch.StartNew();
        long lastFrameEnd = loopStopwatch.ElapsedMilliseconds;
        var targetIntervalMs = 1000.0 / 120.0; // target fps

        while (!cancellationToken.IsCancellationRequested)
        {
            frameStopwatch.Restart();

            // Render DrawnView
            OnFrame?.Invoke(0);

            frameStopwatch.Stop();  

            var frameExecutionTimeMs = frameStopwatch.Elapsed.TotalMilliseconds;
            var elapsedTimeSinceLastFrame = loopStopwatch.ElapsedMilliseconds - lastFrameEnd;
            var timeToWait = targetIntervalMs - elapsedTimeSinceLastFrame - frameExecutionTimeMs;

            if (timeToWait > 0)
                Thread.Sleep(TimeSpan.FromMilliseconds(timeToWait));

            lastFrameEnd = loopStopwatch.ElapsedMilliseconds;
        }
    }

    public static Android.App.Activity MainActivity { get; set; }

    public static void Init(Android.App.Activity activity)
    {
        Initialized = true;

        MainActivity = activity;

        Super.Screen.Density = activity.Resources.DisplayMetrics.Density;

        Super.Screen.WidthDip = activity.Resources.DisplayMetrics.WidthPixels / Super.Screen.Density;
        Super.Screen.HeightDip = activity.Resources.DisplayMetrics.HeightPixels / Super.Screen.Density;

        if (Super.NavBarHeight < 0)
            Super.NavBarHeight = 45; //manual

        //var flags = Activity.Window.Attributes.Flags;

        var isFullscreen = (int)activity.Window.DecorView.SystemUiVisibility & (int)SystemUiFlags.LayoutStable;

        //if (((flags & WindowManagerFlags.TranslucentStatus) == WindowManagerFlags.TranslucentStatus) || isFullscreen>0 || (Activity.Window.StatusBarColor == Android.Graphics.Color.Transparent))
        if (!(isFullscreen > 0))
        {
            Super.StatusBarHeight = GetStatusBarHeight(activity) / Super.Screen.Density;
        }
        else
        {
            Super.StatusBarHeight = 0;
        }

        VisualDiagnostics.VisualTreeChanged += OnVisualTreeChanged;
    }

    /// <summary>
    /// ToDo resolve obsolete for android api 30 and later
    /// </summary>
    /// <param name="activity"></param>
    public static void SetFullScreen(Android.App.Activity activity)
    {
        if (_insetsListener == null)
        {
            _insetsListener = new();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                // https://stackoverflow.com/a/33355089/7149454
                var uiOptions = (int)activity.Window.DecorView.SystemUiVisibility;
                uiOptions |= (int)SystemUiFlags.LayoutStable;
                uiOptions |= (int)SystemUiFlags.LayoutFullscreen;
                activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;
                activity.Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
                var contentView = activity.FindViewById(Android.Resource.Id.Content);
                if (contentView != null)
                    contentView.SetOnApplyWindowInsetsListener(_insetsListener);
            }
        }
    }

    static InsetsListener _insetsListener;

    private static FrameCallback _frameCallback;

    public class InsetsListener : Java.Lang.Object, Android.Views.View.IOnApplyWindowInsetsListener
    {
        private WindowInsets _returnInsets;

        public WindowInsets OnApplyWindowInsets(Android.Views.View v, WindowInsets insets)
        {
            //we are saving system insets BEFORE the fullscreen flag was applied
            //and system insets became zero
            if (_returnInsets == null)
            {
                Super.Screen.TopInset = insets.SystemWindowInsetTop / Super.Screen.Density;

                bool invalidate = false;

                if (Super.StatusBarHeight != Super.Screen.TopInset)
                    invalidate = true;

                Super.StatusBarHeight = Super.Screen.TopInset;

                //if (invalidate)
                {
                    //App.Instance.Shell.InvalidateNavBar();
                    InsetsChanged?.Invoke(this, null);
                }

                _returnInsets = insets.ReplaceSystemWindowInsets(
                    insets.SystemWindowInsetLeft,
                    0,
                    insets.SystemWindowInsetRight,
                    insets.SystemWindowInsetBottom
                );
            }
            return _returnInsets;
        }
    }

    public static int GetStatusBarHeight(Context context)
    {

        int statusBarHeight = 0, totalHeight = 0, contentHeight = 0;
        int resourceId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
        if (resourceId > 0)
        {
            statusBarHeight = context.Resources.GetDimensionPixelSize(resourceId);
            totalHeight = context.Resources.DisplayMetrics.HeightPixels;
            contentHeight = totalHeight - statusBarHeight;
            statusBarHeight = statusBarHeight;
        }

        return statusBarHeight;
    }

    //public static void ClearImagesCache()
    //{
    //    var glide = Glide.Get(Platform.CurrentActivity);
    //    Task.Run(async () =>
    //    {
    //        glide.ClearDiskCache();
    //    }).ConfigureAwait(false);

    //    MainThread.BeginInvokeOnMainThread(() =>
    //    {
    //        glide.ClearMemory();
    //    });
    //}


    public static void SetNavigationBarColor(
        Microsoft.Maui.Graphics.Color colorBar,
        Microsoft.Maui.Graphics.Color colorSeparator,
        bool darkStatusBarTint)
    {
        if (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Lollipop)
            return;

        var activity = Platform.CurrentActivity;
        var window = activity.Window;

        window.ClearFlags(WindowManagerFlags.TranslucentNavigation);
        window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
        {
            if (darkStatusBarTint)
            {
                if (Build.VERSION.SdkInt > (BuildVersionCodes)27)
                {
                    if (colorSeparator == Colors.Transparent)
                        window.NavigationBarDividerColor =
                            Microsoft.Maui.Graphics.Color.FromArgb("#FF222222").ToAndroid();
                    else
                        window.NavigationBarDividerColor = colorSeparator.ToAndroid();
                }

                window.SetNavigationBarColor(colorBar.ToAndroid());

                if (Build.VERSION.SdkInt > (BuildVersionCodes)26)
                {
                    // Fetch the current flags.
                    var lFlags = activity.Window.DecorView.SystemUiVisibility;

                    var mask = ~(StatusBarVisibility)SystemUiFlags.LightNavigationBar;

                    // Update the SystemUiVisibility dependening on whether we want a Light or Dark theme.
                    activity.Window.DecorView.SystemUiVisibility = lFlags & mask;
                }
            }
            else
            {
                //todo share everywhere !!!
                if (Build.VERSION.SdkInt > (BuildVersionCodes)27)
                {
                    if (colorSeparator == Colors.Transparent)
                        window.NavigationBarDividerColor =
                            Microsoft.Maui.Graphics.Color.FromArgb("#FFeeeeee").ToAndroid();
                    else
                        window.NavigationBarDividerColor = colorSeparator.ToAndroid();
                }

                window.SetNavigationBarColor(colorBar.ToAndroid());

                if (Build.VERSION.SdkInt > (BuildVersionCodes)26)
                {

                    // Fetch the current flags.
                    var lFlags = activity.Window.DecorView.SystemUiVisibility;
                    // Update the SystemUiVisibility dependening on whether we want a Light or Dark theme.
                    activity.Window.DecorView.SystemUiVisibility =
                        lFlags | (StatusBarVisibility)SystemUiFlags.LightNavigationBar;
                }

            }
        }
    }

    public static void SetWhiteTextStatusBar()
    {
        if (Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.M)
        {

            var activity = Platform.CurrentActivity;
            var window = activity.Window;

            // Fetch the current flags.
            var lFlags = window.DecorView.SystemUiVisibility;

            var mask = ~(StatusBarVisibility)SystemUiFlags.LightStatusBar;

            window.DecorView.SystemUiVisibility = lFlags & mask;
        }
    }

    public static void SetBlackTextStatusBar()
    {
        if (Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.M)
        {
            var activity = Platform.CurrentActivity;
            var window = activity.Window;

            // Fetch the current flags.
            var lFlags = window.DecorView.SystemUiVisibility;
            // Update the SystemUiVisibility dependening on whether we want a Light or Dark theme.
            window.DecorView.SystemUiVisibility = lFlags | (StatusBarVisibility)SystemUiFlags.LightStatusBar;
        }
    }

    public class FrameCallback : Java.Lang.Object, IFrameCallback
    {
        public FrameCallback(Action<long> callback)
        {
            _callback = callback;
        }

        Action<long> _callback;

        public void DoFrame(long frameTimeNanos)
        {
            _callback?.Invoke(frameTimeNanos);
        }

    }
}
