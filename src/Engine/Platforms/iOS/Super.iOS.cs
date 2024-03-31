using CoreAnimation;
using Foundation;
using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using SkiaSharp.Views.iOS;
using System.Diagnostics;
using UIKit;
using Platform = Microsoft.Maui.ApplicationModel.Platform;

namespace DrawnUi.Maui.Draw
{

    public partial class Super
    {

        #region Thread
        static bool PlatformIsMainThread
        {
            get
            {
                return false;
            }
        }

        static void PlatformBeginInvokeOnMainThread(Action action, string Identifier = null)
        {

        }

        #endregion

        public static void Init()
        {
            if (Initialized)
                return;

            Initialized = true;

            Super.Screen.Density = UIScreen.MainScreen.Scale;
            Super.Screen.WidthDip = UIScreen.MainScreen.Bounds.Width;
            Super.Screen.HeightDip = UIScreen.MainScreen.Bounds.Height;

            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                var window = new UIWindow(frame: UIScreen.MainScreen.Bounds)
                { BackgroundColor = Colors.Transparent.ToUIColor() };

                Super.Screen.TopInset = (int)(window.SafeAreaInsets.Top);
                Super.Screen.BottomInset = (int)(window.SafeAreaInsets.Bottom);
                Super.Screen.LeftInset = (int)(window.SafeAreaInsets.Left);
                Super.Screen.RightInset = (int)(window.SafeAreaInsets.Right);
            }

            Super.StatusBarHeight = Super.Screen.TopInset;
            if (Super.StatusBarHeight <= 0)
                Super.StatusBarHeight = 20;

            if (Super.NavBarHeight < 0)

                Super.NavBarHeight = 47; //manual

            InsetsChanged?.Invoke(null, null);


            
            Tasks.StartDelayed(TimeSpan.FromMilliseconds(500), async () =>
            {
                while (!_loopStarted)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        if (_loopStarting)
                            return;
                        _loopStarting = true;

                        if (MainThread.IsMainThread) //CADisplayLink is available
                        {
                            if (!_loopStarted)
                            {
                                _loopStarted = true;
                                try
                                {
                                    _displayLink = CADisplayLink.Create(OnFrame);
                                    _displayLink.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Common);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    throw;
                                }
                            }
                        }
                        _loopStarting = false;
                    });
                    await Task.Delay(100);
                }
            });
            
        }
    
        static bool _loopStarting = false;
        static bool _loopStarted = false;

        static void OnFrame()
        {
            DisplayLinkCallback?.Invoke(null, null);
        }

        public static event EventHandler DisplayLinkCallback;
        static CADisplayLink _displayLink;

        public static UINavigationController NavigationController { get; set; } = null;

        public static UIStatusBarStyle? OrderedStyle { get; set; }

        public static void SetBlackTextStatusBar()
        {
            Debug.WriteLine("[StatusBar] BLACK");

            var controller = Platform.GetCurrentUIViewController();

            if (controller == null || controller.NavigationController == null)
            {
                OrderedStyle = UIStatusBarStyle.DarkContent;

                UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.DarkContent, false);
                controller.SetNeedsStatusBarAppearanceUpdate();

            }
            else
            {
                OrderedStyle = null;
                controller.NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
            }
        }

        public static void SetWhiteTextStatusBar()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Update the UI
                Debug.WriteLine("[StatusBar] WHITE");

                var controller = Platform.GetCurrentUIViewController();
                if (controller == null || controller.NavigationController == null)
                {
                    OrderedStyle = UIStatusBarStyle.LightContent;

                    UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
                    controller.SetNeedsStatusBarAppearanceUpdate();
                }
                else
                {
                    OrderedStyle = null;
                    controller.NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
                }
            });



        }

    }
}
