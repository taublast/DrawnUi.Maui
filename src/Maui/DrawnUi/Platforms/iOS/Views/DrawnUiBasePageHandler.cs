using CoreGraphics;
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace DrawnUi.Controls;

public class DrawnUiBasePageHandler : PageHandler
{

    protected override void ConnectHandler(Microsoft.Maui.Platform.ContentView platformView)
    {
        base.ConnectHandler(platformView);

        UnregisterForKeyboardNotifications();
        RegisterForKeyboardNotifications();

    }

    protected override void DisconnectHandler(Microsoft.Maui.Platform.ContentView platformView)
    {
        base.DisconnectHandler(platformView);

        UnregisterForKeyboardNotifications();
    }

    NSObject _keyboardShowObserver;
    NSObject _keyboardHideObserver;
    private UIStatusBarStyle? _statusStyle;

    void RegisterForKeyboardNotifications()
    {
        if (_keyboardShowObserver == null)
            _keyboardShowObserver = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardShow);
        if (_keyboardHideObserver == null)
            _keyboardHideObserver = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardHide);
    }

    void OnKeyboardShow(object sender, UIKeyboardEventArgs args)
    {

        NSValue result = (NSValue)args.Notification.UserInfo.ObjectForKey(new NSString(UIKeyboard.FrameEndUserInfoKey));
        CGSize keyboardSize = result.RectangleFValue.Size;
        if (VirtualView is DrawnUiBasePage control)
        {
            control.KeyboardResized(keyboardSize.Height);
            //Element.Margin = new Thickness(0, 0, 0, keyboardSize.Height); //push the entry up to keyboard height when keyboard is activated
        }
    }

    void OnKeyboardHide(object sender, UIKeyboardEventArgs args)
    {
        if (VirtualView is DrawnUiBasePage control)
        {
            control.KeyboardResized(0.0);
            //Element.Margin = new Thickness(0); //set the margins to zero when keyboard is dismissed
        }
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
}
