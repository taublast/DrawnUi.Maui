using AppoMobi.Maui.Gestures;
using AppoMobi.Specials;
using CoreAnimation;
using DrawnUi.Draw;
using ExCSS;
using Foundation;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Color = Xamarin.Forms.Color;

[assembly: Xamarin.Forms.Dependency(typeof(AppoMobi.Xamarin.DrawnUi.Apple.DrawnUi))]
namespace AppoMobi.Xamarin.DrawnUi.Apple
{
    [global::Xamarin.Forms.Internals.Preserve(AllMembers = true)]
    public class DrawnUi : IDrawnUiPlatform
    {
        public DrawnUi()
        {

        }

        public static void Initialize<T>() where T : Application
        {
            Super.AppAssembly = typeof(T).Assembly;

            TouchEffect.Density = (float)UIScreen.MainScreen.Scale;

            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                var window = new UIWindow(frame: UIScreen.MainScreen.Bounds)
                { BackgroundColor = Color.Transparent.ToUIColor() };

                Super.StatusBarHeight = (int)(window.SafeAreaInsets.Top);
                Super.BottomInsets = (int)(window.SafeAreaInsets.Bottom);
            }

            if (Super.StatusBarHeight <= 0)
                Super.StatusBarHeight = 20;

            if (Super.NavBarHeight < 0)
                Super.NavBarHeight = 47; //manual

            Super.InsetsChanged?.Invoke(null, null);

            Tasks.StartDelayed(TimeSpan.FromMilliseconds(250), async () =>
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

        public static event EventHandler DisplayLinkCallback;
        static CADisplayLink _displayLink;

        static void OnFrame()
        {
            DisplayLinkCallback?.Invoke(null, null);
        }

        public static bool DisableCache;

        async Task<SKBitmap> IDrawnUiPlatform.LoadImageOnPlatformAsync(ImageSource source, CancellationToken cancel)
        {
            if (source == null)
                return null;

            UIImage iosImage = null;
            try
            {
                if (source is UriImageSource uri)
                {
                    using var client = new WebClient();
                    var data = await client.DownloadDataTaskAsync(uri.Uri);
                    return SKBitmap.Decode(data);
                }

                var handler = GetHandler(source);
                iosImage = await handler.LoadImageAsync(source, cancel, 1.0f);

                if (iosImage != null)
                {
                    return iosImage.ToSKBitmap();
                }
            }
            catch (Exception e)
            {
                Super.Log($"[LoadSKBitmapAsync] {e}");
            }

            return null;
        }

        void IDrawnUiPlatform.ClearImagesCache()
        {
            //NukeHelper.ClearCache();
        }

        public void RegisterLooperCallback(EventHandler callback)
        {
            DisplayLinkCallback += callback;
        }

        public void UnregisterLooperCallback(EventHandler callback)
        {
            DisplayLinkCallback -= callback;
        }

        public bool CheckNativeVisibility(object handler)
        {

            if (handler != null)
            {
                if (handler is UIKit.UIView iosView)
                {
                    if (iosView.Hidden)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static IImageSourceHandler? GetHandler(ImageSource source)
        {
            //Image source handler to return
            IImageSourceHandler? returnValue = null;
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
}