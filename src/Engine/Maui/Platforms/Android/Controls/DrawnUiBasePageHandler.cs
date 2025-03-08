using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace DrawnUi.Maui.Controls;

public class DrawnUiBasePageHandler : PageHandler
{
    private MyLayoutListener _keyboardShowObserver;

    private void UnregisterForKeyboardNotifications()
    {
        if (_keyboardShowObserver != null)
        {
            _keyboardShowObserver.Release();
            _keyboardShowObserver = null;
        }
    }

    private void RegisterForKeyboardNotifications(ContentViewGroup platformView)
    {
        if (platformView != null && VirtualView is DrawnUiBasePage control)
            if (_keyboardShowObserver == null)
            {
                //var rootView = Platform.CurrentActivity.Window.DecorView;
                var rootView = platformView;

                _keyboardShowObserver = new MyLayoutListener(rootView, control);
            }
    }

    protected override void ConnectHandler(ContentViewGroup platformView)
    {
        base.ConnectHandler(platformView);

        UnregisterForKeyboardNotifications();
        RegisterForKeyboardNotifications(platformView);
    }

    protected override void DisconnectHandler(ContentViewGroup platformView)
    {
        base.DisconnectHandler(platformView);

        UnregisterForKeyboardNotifications();
    }
}