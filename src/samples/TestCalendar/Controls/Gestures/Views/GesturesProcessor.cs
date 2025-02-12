using AppoMobi.Maui.Gestures;

namespace AppoMobi.Touch;

public class GesturesProcessor
{
    public static bool OnGestureEvent(
        IWithTouch parent,
        TouchActionType type, TouchActionEventArgs args, TouchActionResult action, bool isDrawn = false)
    {

        if (isDrawn)
        {
            var consumed = false;
            switch (action)
            {
                case TouchActionResult.Down:
                    consumed = parent.GestureHandler.OnDown(new DownUpEventArgs(args));
                    break;
                case TouchActionResult.Up:
                    consumed = parent.GestureHandler.OnUp(new DownUpEventArgs(args));
                    break;
                case TouchActionResult.Tapped:
                    consumed = parent.GestureHandler.OnTapped(new TapEventArgs(args));
                    break;
            }
            return consumed;
        }

        switch (action)
        {
            case TouchActionResult.Down:
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    parent.GestureHandler.OnDown(new DownUpEventArgs(args));
                });
                break;
            case TouchActionResult.Up:
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    parent.GestureHandler.OnUp(new DownUpEventArgs(args));
                });
                break;
            case TouchActionResult.Tapped:
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    parent.GestureHandler.OnTapped(new TapEventArgs(args));
                });
                break;
        }

        return true;
    }
}