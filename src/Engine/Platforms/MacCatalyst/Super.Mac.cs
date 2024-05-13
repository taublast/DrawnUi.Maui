using CoreAnimation;
using Foundation;
using Microsoft.Maui.Controls;
using UIKit;

namespace DrawnUi.Maui.Draw
{
    public partial class Super
    {

        private static UIResponder mainResponder;

        public static void RequestMainResponder(UIResponder responder, bool force = false)
        {
            if (mainResponder == null || force)
            {
                mainResponder = responder;
            }

            if (responder == mainResponder)
            {
                responder.BecomeFirstResponder();
            }
        }

        /*
        public static void AttachKeysLayer()
        {
            var root = UIKit.UIApplication.SharedApplication.KeyWindow.RootViewController as Microsoft.Maui.Platform.ContainerViewController;
            var mainPage = root?.CurrentView;
            var nativeView = mainPage?.Handler?.PlatformView as UIKit.UIView;

            if (nativeView.Subviews.All(x => !(x is KeysInputCatchetLayer)))
            {
                if (_layerCatchInput == null)
                {
                    _layerCatchInput = new KeysInputCatchetLayer();
                }
                _layerCatchInput.Frame = nativeView.Frame;
                nativeView.AddSubview(_layerCatchInput);
            }
            _layerCatchInput?.BecomeFirstResponder();
        }

        static KeysInputCatchetLayer _layerCatchInput;
        */

        public static void TrackKeyboardKeys()
        {

        }

        NSObject _keyboardShowObserver;
        NSObject _keyboardHideObserver;

        void RegisterForKeyboardNotifications()
        {
            if (_keyboardShowObserver == null)
                _keyboardShowObserver = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardShow);
            if (_keyboardHideObserver == null)
                _keyboardHideObserver = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardHide);
        }
        void OnKeyboardShow(object sender, UIKeyboardEventArgs args)
        {
            //NSValue result = (NSValue)args.Notification.UserInfo.ObjectForKey(new NSString(UIKeyboard.FrameEndUserInfoKey));
            //CGSize keyboardSize = result.RectangleFValue.Size;
            //if (Element != null)
            //{
            //    ((NiftyPage)Element).KeyboardResized(keyboardSize.Height);
            //}
        }
        void OnKeyboardHide(object sender, UIKeyboardEventArgs args)
        {
            //if (Element != null)
            //{
            //    ((NiftyPage)Element).KeyboardResized(0.0);
            //}
        }

        void UnregisterForKeyboardNotifications()
        {
            if (_keyboardShowObserver != null)
            {
                _keyboardShowObserver.Dispose();
                _keyboardShowObserver = null;
            }

            if (_keyboardHideObserver != null)
            {
                _keyboardHideObserver.Dispose();
                _keyboardHideObserver = null;
            }
        }







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

            if (Super.NavBarHeight < 0)

                Super.NavBarHeight = 47; //manual

            if (UseDisplaySync)
            {

                Tasks.StartDelayed(TimeSpan.FromMilliseconds(250), async () =>
                {
                    while (!_loopStarted)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            if (_loopStarting)
                                return;
                            _loopStarting = true;

                            if (MainThread.IsMainThread) //Choreographer is available
                            {
                                if (!_loopStarted)
                                {
                                    _loopStarted = true;
                                    try
                                    {
                                        _displayLink = CADisplayLink.Create(() => OnFrame?.Invoke(null, null));
                                        _displayLink.AddToRunLoop(NSRunLoop.Current, NSRunLoopMode.Default);
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
            else
            {
                Looper = new(() =>
                {
                    OnFrame?.Invoke(null, null);
                });

                Looper.StartOnMainThread(120);
            }
        }

        static bool _loopStarting = false;
        static bool _loopStarted = false;

        static Looper Looper { get; set; }

        /// <summary>
        /// When set to true will run loop upon CADisplayLink hits instead of a timer looper that targets 120 fps
        /// </summary>
        public static bool UseDisplaySync = false;

        public static event EventHandler OnFrame;

        static CADisplayLink _displayLink;

    }

}