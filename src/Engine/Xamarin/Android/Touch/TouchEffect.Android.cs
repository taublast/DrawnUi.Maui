using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Views.InputMethods;
using Xamarin.Essentials;

namespace AppoMobi.Maui.Gestures
{
    /*
    public partial class TouchEffect
    {
        static bool _closingKeyboard;
        public static void ClosePlatformKeyboard()
        {
            if (!_closingKeyboard)
            {
                _closingKeyboard = true;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(250); // For some reason, a short delay is required here.
                    try
                    {
                        var imm = (InputMethodManager)Platform.AppContext.GetSystemService(Context.InputMethodService);
                        var token = Platform.CurrentActivity?.Window?.DecorView?.WindowToken;
                        imm.HideSoftInputFromWindow(token, 0);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        _closingKeyboard = false;
                    }

                });
            }
        }

    }
    */
}
