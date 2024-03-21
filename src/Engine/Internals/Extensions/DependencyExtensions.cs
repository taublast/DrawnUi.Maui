using AppoMobi.Specials;
using DrawnUi.Maui.Controls;
using EasyCaching.InMemory;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace DrawnUi.Maui.Draw;

public class UiSettings
{
    /// <summary>
    /// For desktop: if set will affect the app window at startup.
    /// </summary>
    public WindowParameters? DesktopWindow { get; set; }

    /// <summary>
    /// Avoid safe insets and remove system ui like status bar etc if supported by platform
    /// </summary>
    public bool MobileIsFullscreen { get; set; }

    /// <summary>
    /// Listen to desktop keyboard keys with KeyboardManager. Windows and Catalyst available.
    /// </summary>
    public bool UseDesktopKeyboard { get; set; }
}

public struct WindowParameters
{
    public int Width { get; set; }

    public int Height { get; set; }

    /// <summary>
    /// For desktop: if you set this to true the app window will not be allowed to be resized manually.
    /// </summary>
    public bool IsFixedSize { get; set; }
}

public static class DependencyExtensions
{
    public static UiSettings StartupSettings { get; set; }

    public static MauiAppBuilder UseDrawnUi(this MauiAppBuilder builder, UiSettings settings = null)
    {
        StartupSettings = settings;

        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID
            handlers.AddHandler(typeof(DrawnUiBasePage), typeof(DrawnUiBasePageHandler));
            handlers.AddHandler(typeof(MauiEntry), typeof(MauiEntryHandler));
            handlers.AddHandler(typeof(MauiEditor), typeof(MauiEditorHandler));
#elif IOS
            handlers.AddHandler(typeof(MauiEntry), typeof(MauiEntryHandler));
            handlers.AddHandler(typeof(MauiEditor), typeof(MauiEditorHandler));

            //if (DeviceInfo.Current.DeviceType != DeviceType.Virtual)
            {
                handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(SKMetalViewRenderer));
            }
#elif MACCATALYST
            handlers.AddHandler(typeof(MauiEntry), typeof(MauiEntryHandler));
            handlers.AddHandler(typeof(MauiEditor), typeof(MauiEditorHandler));
            handlers.AddHandler(typeof(DrawnUiBasePage), typeof(DrawnUiBasePageHandler));
            handlers.AddHandler(typeof(SkiaViewAccelerated), typeof(SKMetalViewRenderer));
            //handlers.AddHandler(typeof(Window), typeof(CustomizedWindowHandler));
            
#elif WINDOWS
            handlers.AddHandler(typeof(MauiEntry), typeof(MauiEntryHandler));
            handlers.AddHandler(typeof(MauiEditor), typeof(MauiEditorHandler));
#endif
        });


        builder
            .ConfigureAnimations();

        builder
            .UseSkiaSharp(true);

        builder.UseGestures();

        //In-Memory Caching of bitmaps
        builder.Services  //Important step for In-Memory Caching
            .AddEasyCaching(options =>
            {

                // use memory cache with your own configuration
                options.UseInMemory(config =>
                {
                    config.DBConfig = new InMemoryCachingOptions
                    {
                        // scan time, default value is 60s
                        ExpirationScanFrequency = 300,
                        // total count of cache items, default value is 10000
                        SizeLimit = 100,

                        // below two settings are added in v0.8.0
                        // enable deep clone when reading object from cache or not, default value is true.
                        EnableReadDeepClone = false,
                        // enable deep clone when writing object to cache or not, default valuee is false.
                        EnableWriteDeepClone = false,
                    };
                    // the max random second will be added to cache's expiration, default value is 120
                    config.MaxRdSecond = SkiaImageManager.CacheLongevitySecs;
                    // whether enable logging, default is false
                    config.EnableLogging = false;
                    // mutex key's alive time(ms), default is 5000
                    config.LockMs = 5000;
                    // when mutex key alive, it will sleep some time, default is 300
                    config.SleepMs = 300;
                }, "skiaimages");
            });

        Super.OnNativeAppCreated += (s, a) =>
        {

            Super.App = Super.Services.GetRequiredService<IApplication>();


            //bug: if you uncomment this you get constant GC collects on android
            //just for refreshing an empty skiasharp view with nothing there
            //while the callback never even gets invoked

            //DeviceDisplay.MainDisplayInfoChanged += (s, e) =>
            //{
            //    Super.Screen.Density = DeviceDisplay.Current.MainDisplayInfo.Density;
            //};

        };

        Super.OnMauiAppCreated += () =>
        {
            //todo: not optimal  
            Tasks.StartDelayed(TimeSpan.FromSeconds(2), () =>
            {
                //close keyboard if any
                TouchEffect.CloseKeyboard();
            });
        };

        void InvokeLifecycleEvents<TDelegate>(Action<TDelegate> action)
           where TDelegate : Delegate
        {
            if (Super.Services == null)
                return;

            var delegates = GetLifecycleEventDelegates<TDelegate>();

            foreach (var del in delegates)
                action?.Invoke(del);
        }

        IEnumerable<TDelegate> GetLifecycleEventDelegates<TDelegate>(string? eventName = null)
            where TDelegate : Delegate
        {
            var lifecycleService = Super.Services?.GetService<ILifecycleEventService>();
            if (lifecycleService == null)
                yield break;

            if (eventName == null)
                eventName = typeof(TDelegate).Name;

            foreach (var del in lifecycleService.GetEventDelegates<TDelegate>(eventName))
                yield return del;
        }


        builder.ConfigureLifecycleEvents(AppLifecycle =>
        {

            //todo
            // 1 every platform sets density

#if WINDOWS

            AppLifecycle.AddEvent<WindowsLifecycle.OnWindowCreated>("OnWindowCreated",
                (window) =>
                {
                    Super.Init();
                    Super.OnMauiAppCreated?.Invoke();
                });

            AppLifecycle.AddEvent<WindowsLifecycle.OnLaunched>("OnLaunched",
                (Microsoft.UI.Xaml.Application application, Microsoft.UI.Xaml.LaunchActivatedEventArgs args) =>
                {
                    Super.App = Super.Services.GetRequiredService<IApplication>();

                    var appWindow = Super.App.Windows.First() as Microsoft.Maui.Controls.Window;

                    var window = appWindow.Handler?.PlatformView as Microsoft.Maui.MauiWinUIWindow;

                    Super.Screen.Density = DeviceDisplay.Current.MainDisplayInfo.Density;

                    appWindow.DisplayDensityChanged += (sender, args) =>
                    {
                        Super.Screen.Density = args.DisplayDensity; //works fine when changing display
                    };

                    if (StartupSettings != null)
                    {
                        if (StartupSettings.UseDesktopKeyboard)
                        {
                            KeyboardManager.AttachToKeyboard(window.Content);
                        }

                        if (StartupSettings.DesktopWindow != null)
                        {
                            Super.ResizeWindow(appWindow,
                                StartupSettings.DesktopWindow.Value.Width,
                                StartupSettings.DesktopWindow.Value.Height,
                                StartupSettings.DesktopWindow.Value.IsFixedSize);
                        }
                    }

                    //MainPage
                    //var page = appWindow.Page;

                    //App created
                    //var a = window.GetAppWindow();
                    //var win = window.GetWindow();
                    //var handle = window.GetWindowHandle();



                    Super.OnCreated();
                });



#elif ANDROID

            AppLifecycle.AddAndroid((android) =>
            {
                android.OnCreate((activity, bundle) =>
                {
                    Super.Init(activity);

                    if (StartupSettings != null)
                    {
                        if (StartupSettings.MobileIsFullscreen)
                        {
                            Super.SetFullScreen(activity);
                        }
                    }
                    Super.OnMauiAppCreated?.Invoke();
                });

                android.OnApplicationCreate((app) =>
                {
                    Super.OnCreated();
                });


                ActivityState activityState = ActivityState.Destroyed;

                Platform.ActivityStateChanged += (o, args) =>
                {
                    if (activityState != args.State)
                    {
                        if ((args.State == ActivityState.Resumed || args.State == ActivityState.Started)
                            && activityState != ActivityState.Resumed && activityState != ActivityState.Started)
                        {
                            //Console.WriteLine("[APP] OnResumed");
                            Super.OnWentForeground();
                        }
                        else
                        if ((args.State == ActivityState.Paused || args.State == ActivityState.Stopped)
                            && activityState != ActivityState.Paused && args.State != ActivityState.Stopped)
                        {
                            //Console.WriteLine("[APP] OnPause");
                            Super.OnWentBackground();
                        }
                        activityState = args.State;
                    }
                };

                //android.OnBackPressed((activity) =>
                //{
                //    //todo use settings?
                //});

                android.OnNewIntent((activity, intent) =>
                {
                    if (StartupSettings != null)
                    {
                        if (StartupSettings.MobileIsFullscreen)
                        {
                            Super.SetFullScreen(activity);
                        }
                    }
                });
            });


#elif IOS || MACCATALYST

            AppLifecycle.AddiOS((apple) =>
            {

                apple.DidEnterBackground((app) =>
                {
                    Super.OnWentBackground();
                });

                apple.WillEnterForeground((app) =>
                {
                    Super.OnWentForeground();
                });

                bool onceApple = false;
                apple.OnActivated((del) =>
                {
                    if (!onceApple)
                    {
                        onceApple = true;
                        Super.Init();
                        Super.OnMauiAppCreated?.Invoke();
                    }
                });
                
                apple.FinishedLaunching((application, launchOptions) =>
                {
                    Super.App = Super.Services.GetRequiredService<IApplication>();
                    var appWindow = Super.App.Windows.First() as Microsoft.Maui.Controls.Window;
                    var view = appWindow.Handler?.PlatformView as UIKit.UIView;
                    //var check = UIKit.UIApplication.SharedApplication.KeyWindow;

#if MACCATALYST

                    Foundation.NSNotificationCenter.DefaultCenter.AddObserver("NSWindowDidBecomeMainNotification", null, null,
                              (h) =>
                              {
                                  if (UIKit.UIApplication.SharedApplication.KeyWindow.RootViewController is Microsoft.Maui.Platform.ContainerViewController container)
                                  {
                                      if (container.CurrentView is DrawnUiBasePage page)
                                      {
                                          if (page.Handler is DrawnUiBasePageHandler handler)
                                          {
                                              Super.RequestMainResponder(handler.PlatformView);
                                          }
                                      }
                                  }
                                  // else if (UIKit.UIApplication.SharedApplication.KeyWindow.RootViewController is
                                  //          Microsoft.Maui.Controls.Platform.Compatibility.ShellFlyoutRenderer shell)
                                  // {
                                  //     //unsupported
                                  // }
                                  
                                  
                              });

#endif

                    Foundation.NSNotificationCenter.DefaultCenter.AddObserver("NSWindowDidUpdateNotification", null, null,
                        (h) =>
                        {
                            foreach (var scene in UIKit.UIApplication.SharedApplication.ConnectedScenes)
                            {
                                if (scene is UIKit.UIWindowScene windowScene)
                                {

                                    if (Super.Screen.Density != windowScene.Screen.Scale)
                                    {
                                        //not working with several displays never ever
                                        //var test = DeviceDisplay.Current.MainDisplayInfo.Density;                                      
                                        Super.Screen.Density = windowScene.Screen.Scale;
                                        Super.NeedGlocalUpdate();
                                    }
                                }
                            }
                        });

                    if (StartupSettings != null)
                    {
#if MACCATALYST


                        //might be implemented too..
                        //NSWindowDidChangeScreenNotification
                        //NSWindowDidResizeNotification
                        //
                        //NSWindowDidChangeBackingPropertiesNotification

                        if (StartupSettings.UseDesktopKeyboard)
                        {

                            Foundation.NSNotificationCenter.DefaultCenter.AddObserver("NSWindowDidChangeBackingPropertiesNotification", null, null,
                                (h) =>
                                {
                                    foreach (var scene in UIKit.UIApplication.SharedApplication.ConnectedScenes)
                                    {
                                        if (scene is UIKit.UIWindowScene windowScene)
                                        {

                                            if (Super.Screen.Density != windowScene.Screen.Scale)
                                            {
                                                Super.Screen.Density = windowScene.Screen.Scale;
                                                Super.NeedGlocalUpdate();
                                            }
                                        }
                                    }

                                });
                        }

                        if (StartupSettings.DesktopWindow != null)
                        {
                            Super.ResizeWindow(appWindow,
                                StartupSettings.DesktopWindow.Value.Width,
                                StartupSettings.DesktopWindow.Value.Height,
                                StartupSettings.DesktopWindow.Value.IsFixedSize);
                        }
#elif IOS
                        if (StartupSettings.MobileIsFullscreen)
                        {
                            var mainPage = appWindow.Page;
                            Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page.SetUseSafeArea(mainPage, false);
                        }
#endif
                    }

                    Super.OnCreated();



                    //if (StartupSettings != null)
                    //{

                    //}

                    return true;

                });

            });

#endif

        });

        // Microsoft.Maui.Handlers.ViewHandler.ViewMapper.AppendToMapping(nameof(Application.Resources), (handler, view) =>
        // {
        //     if (!mauiCreated)
        //     {
        //         mauiCreated = true;
        //         Super.OnMauiAppCreated?.Invoke();
        //
        //
        //     }
        // });

        return builder;
    }

    private static bool mauiCreated;
}